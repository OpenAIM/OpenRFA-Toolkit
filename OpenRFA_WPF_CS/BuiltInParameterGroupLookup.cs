using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit;
using Autodesk.Revit.DB;

namespace OpenRFA_WPF_CS
{
    /// <summary>
    /// Allows you to lookup a BuiltInParameterGroup by passing in a group name string.
    /// From: https://forums.autodesk.com/t5/revit-api-forum/shared-parameters-changing-the-group-parameter-under/td-p/3221342
    /// </summary>
    public class BuiltInParameterGroupLookup
    {
        private Dictionary<string, BuiltInParameterGroup> _lookup;

        public BuiltInParameterGroupLookup()
        {
            _lookup = new Dictionary<string, BuiltInParameterGroup>();
            BuildLookup();
        }

        /// <summary>
        /// Gets the BuiltInParameterGroup enum value that corresponds to the passed in label.
        /// If a corresponding enum value is not present, INVALID will be returned.
        /// </summary>
        /// <param name="label">The group name that is shown to the user in the Revit UI.</param>
        /// <returns>Returns the corresponding BuiltInParameterGroup.</returns>
        public BuiltInParameterGroup this[string label]
        {
            get
            {
                if (_lookup.ContainsKey(label))
                {
                    return _lookup[label];
                }
                else
                {
                    return BuiltInParameterGroup.INVALID;
                }
            }
        }

        /// <summary>
        /// Initializes the lookup list using jwthe Revit API "LabelUtils" object.
        /// </summary>
        private void BuildLookup()
        {
            var values = Enum.GetValues(typeof(BuiltInParameterGroup));
            foreach (BuiltInParameterGroup bipg in values)
            {
                string label = LabelUtils.GetLabelFor(bipg);
                if (!_lookup.ContainsKey(label))
                {
                    _lookup.Add(label, bipg);
                }
            }
        }
    }
}
