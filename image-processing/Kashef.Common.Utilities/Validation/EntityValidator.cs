using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kashef.Common.Utilities.Validation
{
    /// <summary>
    /// Validator which responsible about validate specific entity against it's metadata.
    /// </summary>
    [DataContract(IsReference=true)]
    [KnownType(typeof(EntityValidator))]
    public class EntityValidator : IEntityValidator
    {
        /// <summary>
        /// Return any error ocuured during the entity validation
        /// </summary>
        public string Error
        {
            get
            {
                TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(this.GetType()), this.GetType());
                StringBuilder b = new StringBuilder();
                var context = new ValidationContext(this, serviceProvider: null, items: null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var isValid = Validator.TryValidateObject(this, context, results, true);
                if (!isValid)
                {
                    foreach (var validationResult in results)
                    {
                        b.AppendLine(validationResult.ErrorMessage);
                    }
                }

                return b.ToString();
            }
        }
        /// <summary>
        /// Validate specific proprty in entity
        /// </summary>
        /// <param name="columnName">proprty name</param>
        /// <returns></returns>
        public string this[string columnName]
        {
            get
            {
                TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(this.GetType()), this.GetType());
                StringBuilder b = new StringBuilder();
                var context = new ValidationContext(this, serviceProvider: null, items: null) { MemberName = columnName };
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                foreach (var itm in this.GetType().GetProperties())
                {
                    if (itm.Name == columnName)
                    {
                        var isValid = Validator.TryValidateProperty(itm.GetValue(this, null), context, results);
                        if (!isValid)
                        {
                            foreach (var validationResult in results)
                            {
                                b.AppendLine(validationResult.ErrorMessage);
                            }
                        }
                    }
                }

                return b.ToString();
            }
        }
        /// <summary>
        /// Check if the entity is valid based on entity's metadata
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        { 
            return string.IsNullOrEmpty(this.Error);
        }
        /// <summary>
        /// Return validation error messages
        /// </summary>
        /// <returns></returns>
        public string GetInvalidMessages()
        {
            return this.Error;
        }
    }
}
