using System;
using System.Collections.Generic;

using UnityEditor.IMGUI.Controls;
using UnityEngine;

using PlasticGui;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Views.Changesets
{
    internal enum ChangesetsListColumn
    {
        Name,
        CreationDate,
        CreatedBy,
        Comment,
        Branch,
        Repository,
        Guid
    }

    [Serializable]
    internal class ChangesetsListHeaderState : MultiColumnHeaderState, ISerializationCallbackReceiver
    {
        internal static ChangesetsListHeaderState Default
        {
            get
            {
                var headerState = new ChangesetsListHeaderState(new Column[]
                {
                    new Column()
                    {
                        width = UnityConstants.ChangesetsColumns.CHANGESET_NUMBER_WIDTH,
                        minWidth = UnityConstants.ChangesetsColumns.CHANGESET_NUMBER_MIN_WIDTH,
                        headerContent = new GUIContent(
                            GetColumnName(ChangesetsListColumn.Name)),
                        allowToggleVisibility = false
                    },
                    new Column()
                    {
                        width = UnityConstants.ChangesetsColumns.CREATION_DATE_WIDTH,
                        minWidth = UnityConstants.ChangesetsColumns.CREATION_DATE_MIN_WIDTH,
                        headerContent = new GUIContent(
                            GetColumnName(ChangesetsListColumn.CreationDate))
                    },
                    new Column()
                    {
                        width = UnityConstants.ChangesetsColumns.CREATED_BY_WIDTH,
                        minWidth = UnityConstants.ChangesetsColumns.CREATED_BY_MIN_WIDTH,
                        headerContent = new GUIContent(
                            GetColumnName(ChangesetsListColumn.CreatedBy))
                    },
                    new Column()
                    {
                        width = UnityConstants.ChangesetsColumns.COMMENT_WIDTH,
                        minWidth = UnityConstants.ChangesetsColumns.COMMENT_MIN_WIDTH,
                        headerContent = new GUIContent(
                            GetColumnName(ChangesetsListColumn.Comment))
                    },
                    new Column()
                    {
                        width = UnityConstants.ChangesetsColumns.BRANCH_WIDTH,
                        minWidth = UnityConstants.ChangesetsColumns.BRANCH_MIN_WIDTH,
                        headerContent = new GUIContent(
                            GetColumnName(ChangesetsListColumn.Branch))
                    },
                    new Column()
                    {
                        width = UnityConstants.ChangesetsColumns.REPOSITORY_WIDTH,
                        minWidth = UnityConstants.ChangesetsColumns.REPOSITORY_MIN_WIDTH,
                        headerContent = new GUIContent(
                            GetColumnName(ChangesetsListColumn.Repository))
                    },
                    new Column()
                    {
                        width = UnityConstants.ChangesetsColumns.GUID_WIDTH,
                        minWidth = UnityConstants.ChangesetsColumns.GUID_MIN_WIDTH,
                        headerContent = new GUIContent(
                            GetColumnName(ChangesetsListColumn.Guid))
                    }
                });

                // NOTE(rafa): we cannot ensure that the order in the list is the same as in the enum
                // take extra care modifying columns list or the enum
                if (headerState.columns.Length != Enum.GetNames(typeof(ChangesetsListColumn)).Length)
                    throw new InvalidOperationException("header columns and Column enum must have the same size and order");

                return headerState;
            }
        }

        internal static List<string> GetColumnNames()
        {
            List<string> result = new List<string>();
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.NameColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.CreationDateColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.CreatedByColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.CommentColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.BranchColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.RepositoryColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.GuidColumn));
            return result;
        }

        internal static string GetColumnName(ChangesetsListColumn column)
        {
            switch (column)
            {
                case ChangesetsListColumn.Name:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.NameColumn);
                case ChangesetsListColumn.CreationDate:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.CreationDateColumn);
                case ChangesetsListColumn.CreatedBy:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.CreatedByColumn);
                case ChangesetsListColumn.Comment:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.CommentColumn);
                case ChangesetsListColumn.Branch:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.BranchColumn);
                case ChangesetsListColumn.Repository:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.RepositoryColumn);
                case ChangesetsListColumn.Guid:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.GuidColumn);
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

        ChangesetsListHeaderState(Column[] columns)
            : base(columns)
        {
            if (mHeaderTitles == null)
                mHeaderTitles = TreeHeaderColumns.GetTitles(columns);
        }

        [SerializeField]
        string[] mHeaderTitles;
    }
}
