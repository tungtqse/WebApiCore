using EntityFramework.DbContextScope.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using WebApiCore.Common;
using WebApiCore.DataModel.Models;
using WebApiCore.Utility;

namespace WebApiCore.DataAccess
{
    public class AuditDbContext: DbContext, IDbContext
    {
        readonly string TransactionId;

        public AuditDbContext(DbContextOptions options) : base(options)
        {
            TransactionId = Convert.ToString(Guid.NewGuid());
        }

        public override int SaveChanges()
        {

            var list = new List<AuditTrail>();
            Guid currentUserId = HttpContextManager.GetUserId(); 
            var currentDateTime = DateTime.Now;
            var dbset = this.Set<AuditTrail>();

            var modifiedEntries = ChangeTracker.Entries()
                .Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified));

            if(modifiedEntries != null)
            {
                foreach(var entry in modifiedEntries)
                {
                    Type entity = entry.Entity.GetType();

                    if(entry.State == EntityState.Added)
                    {
                        #region Add

                        var status = entity.GetProperty(Constant.BaseProperty.StatusId).GetValue(entry.Entity, null);
                        if (status == null || status.Equals(false))
                            entity.GetProperty(Constant.BaseProperty.StatusId).SetValue(entry.Entity, true, null);

                        XElement xml = new XElement("Create");
                        var itemId = (Guid)entity.GetProperty(Constant.AuditTrailProperty.Id).GetValue(entry.Entity, null);
                        var datatable = GetTableName(entity);
                        var auditTrail = new AuditTrail()
                        {
                            ItemId = itemId,
                            TableName = datatable,
                            ModifiedDate = currentDateTime,
                            ModifiedBy = currentUserId,
                            TrackChange = xml.ToString(),
                            TransactionId = TransactionId,
                            StatusId = true,
                            CreatedDate = currentDateTime,
                            CreatedBy = currentUserId
                        };

                        list.Add(auditTrail);

                        #endregion
                    }
                    else
                    {
                        #region Modify

                        #region Config XML
                        var originalValues = entry.OriginalValues.Properties.ToDictionary(pn => pn, pn => entry.OriginalValues[pn]);
                        var currentValues = entry.CurrentValues.Properties.ToDictionary(pn => pn, pn => entry.CurrentValues[pn]);

                        XElement xml = new XElement("Change");

                        foreach (var value in originalValues)
                        {
                            var oldValue = value.Value != null ? value.Value.ToString() : string.Empty;
                            var newValue = currentValues[value.Key] != null ? currentValues[value.Key].ToString() : string.Empty;

                            if (oldValue != newValue)
                            {
                                var field = new XElement("field");
                                var att = new XAttribute("Name", value.Key);
                                field.Add(att);

                                var oldNode = new XElement("OldValue", oldValue);
                                var newNode = new XElement("NewValue", newValue);
                                field.Add(oldNode);
                                field.Add(newNode);
                                xml.Add(field);
                            }
                        }

                        #endregion

                        // Create instance of AuditTrail for edit item
                        // var instanceAuditTrail = Activator.CreateInstance(auditType);
                        var itemId = (Guid)entity.GetProperty(Constant.AuditTrailProperty.Id).GetValue(entry.Entity, null);
                        var datatable = GetTableName(entity);

                        var auditTrail = new AuditTrail()
                        {
                            ItemId = itemId,
                            TableName = datatable,
                            ModifiedDate = currentDateTime,
                            ModifiedBy = currentUserId,
                            TrackChange = xml.ToString(),
                            TransactionId = TransactionId,
                            StatusId = true,
                            CreatedDate = currentDateTime,
                            CreatedBy = currentUserId
                        };

                        // Insert AuditTrail
                        list.Add(auditTrail);

                        #endregion
                    }

                    #region Update System Fields

                    if (entry.State == EntityState.Added
                        && entity.GetProperty(Constant.BaseProperty.CreatedBy) != null && entity.GetProperty(Constant.BaseProperty.CreatedDate) != null
                        && entity.GetProperty(Constant.BaseProperty.ModifiedBy) != null && entity.GetProperty(Constant.BaseProperty.ModifiedDate) != null)
                    {
                        entity.GetProperty(Constant.BaseProperty.CreatedDate).SetValue(entry.Entity, currentDateTime, null);
                        entity.GetProperty(Constant.BaseProperty.CreatedBy).SetValue(entry.Entity, currentUserId, null);
                        entity.GetProperty(Constant.BaseProperty.ModifiedDate).SetValue(entry.Entity, currentDateTime, null);
                        entity.GetProperty(Constant.BaseProperty.ModifiedBy).SetValue(entry.Entity, currentUserId, null);
                    }
                    else if (entry.State == EntityState.Modified
                        && entity.GetProperty(Constant.BaseProperty.ModifiedBy) != null && entity.GetProperty(Constant.BaseProperty.ModifiedDate) != null)
                    {
                        entity.GetProperty(Constant.BaseProperty.ModifiedDate).SetValue(entry.Entity, currentDateTime, null);
                        entity.GetProperty(Constant.BaseProperty.ModifiedBy).SetValue(entry.Entity, currentUserId, null);
                    }

                    #endregion
                }
            }

            try
            {
                var result =  base.SaveChanges();

                try
                {
                    dbset.AddRange(list);

                    base.SaveChanges();
                }
                catch(Exception e)
                {

                }

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private string GetTableName(Type ent)
        {
            string mapping = "";

            var entity = base.Model.FindEntityType(ent.Name);

            if(entity != null)
            {
                mapping = entity.Relational().TableName;
            }
            else
            {
                mapping = base.Model.FindEntityType(ent.FullName).Relational().TableName;
            }

            return mapping;
        }
    }
}
