using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kashef.Common.Utilities.Validation
{
    /// <summary>
    /// Interface to validate entity
    /// </summary>
    public interface IEntityValidator : IDataErrorInfo
    {
        /// <summary>
        /// Check if the entity is valid based on entity's metadata
        /// </summary>
        /// <returns></returns>
        bool IsValid();
        
        /// <summary>
        /// Return validation error messages
        /// </summary>
        /// <returns></returns>
        string GetInvalidMessages();
    }
}
