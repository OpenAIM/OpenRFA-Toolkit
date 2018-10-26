using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace OpenRFA_WPF_CS
{
    public class SPBuiltInGroup
    {
        public static BuiltInParameterGroup GetByName(string _name)
        {
            BuiltInParameterGroup builtIn = new BuiltInParameterGroup();

            string caseSwitch = _name;

            switch (caseSwitch)
            {
                case "Mechanical":
                    builtIn = BuiltInParameterGroup.PG_MECHANICAL;
                    break;
                case "Electrical":
                    builtIn = BuiltInParameterGroup.PG_ELECTRICAL;
                    break;
            }

            return builtIn;
        }

        public class GroupList : List<string>
        {

            // Is there a better way to get all groups?
            public static List<BuiltInParameterGroup> GetAllBuiltInGroups()
            {
                List<BuiltInParameterGroup> ValidGroups = new List<BuiltInParameterGroup>();

                ValidGroups.Add(BuiltInParameterGroup.PG_ANALYSIS_RESULTS);
                ValidGroups.Add(BuiltInParameterGroup.PG_ANALYTICAL_ALIGNMENT);
                ValidGroups.Add(BuiltInParameterGroup.PG_ANALYTICAL_MODEL);
                ValidGroups.Add(BuiltInParameterGroup.PG_CONSTRAINTS);
                ValidGroups.Add(BuiltInParameterGroup.PG_CONSTRUCTION);
                ValidGroups.Add(BuiltInParameterGroup.PG_DATA);
                ValidGroups.Add(BuiltInParameterGroup.PG_GEOMETRY); // Dimensions
                ValidGroups.Add(BuiltInParameterGroup.PG_DIVISION_GEOMETRY);
                ValidGroups.Add(BuiltInParameterGroup.PG_AELECTRICAL);
                ValidGroups.Add(BuiltInParameterGroup.PG_ELECTRICAL_CIRCUITING);
                ValidGroups.Add(BuiltInParameterGroup.PG_ELECTRICAL_LIGHTING);
                ValidGroups.Add(BuiltInParameterGroup.PG_ELECTRICAL_LOADS);
                ValidGroups.Add(BuiltInParameterGroup.PG_ELECTRICAL);
                ValidGroups.Add(BuiltInParameterGroup.PG_ENERGY_ANALYSIS);
                ValidGroups.Add(BuiltInParameterGroup.PG_FIRE_PROTECTION);
                ValidGroups.Add(BuiltInParameterGroup.PG_FORCES);
                ValidGroups.Add(BuiltInParameterGroup.PG_GENERAL);
                ValidGroups.Add(BuiltInParameterGroup.PG_GRAPHICS);
                ValidGroups.Add(BuiltInParameterGroup.PG_GREEN_BUILDING);
                ValidGroups.Add(BuiltInParameterGroup.PG_IDENTITY_DATA);
                ValidGroups.Add(BuiltInParameterGroup.PG_IFC);
                ValidGroups.Add(BuiltInParameterGroup.PG_REBAR_SYSTEM_LAYERS);
                ValidGroups.Add(BuiltInParameterGroup.PG_MATERIALS);
                ValidGroups.Add(BuiltInParameterGroup.PG_MECHANICAL);
                ValidGroups.Add(BuiltInParameterGroup.PG_MECHANICAL_AIRFLOW);
                ValidGroups.Add(BuiltInParameterGroup.PG_MECHANICAL_LOADS);
                ValidGroups.Add(BuiltInParameterGroup.PG_ADSK_MODEL_PROPERTIES);
                ValidGroups.Add(BuiltInParameterGroup.PG_MOMENTS);
                ValidGroups.Add(BuiltInParameterGroup.INVALID); // other
                ValidGroups.Add(BuiltInParameterGroup.PG_OVERALL_LEGEND);
                ValidGroups.Add(BuiltInParameterGroup.PG_PHASING);
                ValidGroups.Add(BuiltInParameterGroup.PG_LIGHT_PHOTOMETRICS);
                ValidGroups.Add(BuiltInParameterGroup.PG_PLUMBING);
                ValidGroups.Add(BuiltInParameterGroup.PG_PRIMARY_END);
                ValidGroups.Add(BuiltInParameterGroup.PG_REBAR_ARRAY);
                ValidGroups.Add(BuiltInParameterGroup.PG_RELEASES_MEMBER_FORCES);
                ValidGroups.Add(BuiltInParameterGroup.PG_SECONDARY_END);
                ValidGroups.Add(BuiltInParameterGroup.PG_SECONDARY_END);
                ValidGroups.Add(BuiltInParameterGroup.PG_SEGMENTS_FITTINGS);
                ValidGroups.Add(BuiltInParameterGroup.PG_SLAB_SHAPE_EDIT);
                ValidGroups.Add(BuiltInParameterGroup.PG_STRUCTURAL);
                ValidGroups.Add(BuiltInParameterGroup.PG_STRUCTURAL_ANALYSIS);
                ValidGroups.Add(BuiltInParameterGroup.PG_TEXT);
                ValidGroups.Add(BuiltInParameterGroup.PG_TITLE);
                ValidGroups.Add(BuiltInParameterGroup.PG_VISIBILITY);

                return ValidGroups;
            }

        }
        public static List<string> GetGroupLabels(List<BuiltInParameterGroup> _groups)
        {
            List<string> groupLabels = new List<string>();
            foreach (BuiltInParameterGroup bipg in _groups)
            {
                groupLabels.Add(LabelUtils.GetLabelFor(bipg));
            }

            return groupLabels;
        }

    }
}