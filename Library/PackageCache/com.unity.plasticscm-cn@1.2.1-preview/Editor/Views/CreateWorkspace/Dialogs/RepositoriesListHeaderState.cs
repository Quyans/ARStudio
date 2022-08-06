using System;
using System.Collections.Generic;

using UnityEditor.IMGUI.Controls;
using UnityEngine;

using PlasticGui;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Views.CreateWorkspace.Dialogs
{
    internal enum RepositoriesListColumn
    {
        Name,
        Server
    }

    [Serializable]
    internal class RepositoriesListHeaderState : MultiColumnHeaderState, ISerializationCallbackReceiver
    {
        internal static RepositoriesListHeaderState Default
        {
            get
            {
                var headerState = new RepositoriesListHeaderState(new Column[]
                {
                    new Column()
                    {
                        width = 320,
                        headerContent = new GUIContent(
                            GetColumnName(RepositoriesListColumn.Name)),
                        minWidth = 200,
                        allowToggleVisibility = false,
                    },
                    new Column()
                    {
                        width = 200,
                        headerContent = new GUIContent(
                            GetColumnName(RepositoriesListColumn.Server)),
                        minWidth = 200,
                        allowToggleVisibility = false,
                    }
                });

                // NOTE(rafa): we cannot ensure that the order in the list is the same as in the enum
                // take extra care modifying columns list or the enum
                if (headerState.columns.Length != Enum.GetNames(typeof(RepositoriesListColumn)).Length)
                    throw new InvalidOperationException("header columns and Column enum must have the same size and order");

                return headerState;
            }
        }

        internal static List<string> GetColumnNames()
        {
            List<string> result = new List<string>();
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.NameColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.ServerColumn));
            return result;
        }

        static string GetColumnName(RepositoriesListColumn column)
        {
            switch (column)
            {
                case RepositoriesListColumn.Name:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.NameColumn);
                case RepositoriesListColumn.Server:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.ServerColumn);
                default:
                    return null;
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (mHeaderTitles != null)
                TreeHeaderColumns.SetTitles(columns, mHeaderTitles);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        RepositoriesListHeaderState(Column[] columns)
            : base(columns)
        {
            if (mHeaderTitles == null)
                mHeaderTitles = TreeHeaderColumns.GetTitles(columns);
        }

        [SerializeField]
        string[] mHeaderTitles;
    }
}
