
namespace Kashef.Common.Utilities.TypeResolution
{
    /// <summary>
    /// Base contract type map configurations
    /// </summary>
    public interface ITypeMapConfigurationBase
    {
        /// <summary>
        /// Get descriptor for this instance. 
        /// <remarks>
        /// This descriptor is not unique string.
        /// </remarks>
        /// </summary>
        string Descriptor { get; }
    }
}
