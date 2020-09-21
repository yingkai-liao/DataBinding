using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeViewExamples;
using UnityEngine;

namespace Joybrick
{
    internal class BindingTreeView : TreeViewWithTreeModel<BindingTreeElement>
    {
        const float kRowHeights = 20f;

        static Texture2D[] s_TestIcons =
        {
            EditorGUIUtility.FindTexture ( "sv_icon_dot4_pix16_gizmo"), //非監聽 有資料的DataPair 黃燈
            EditorGUIUtility.FindTexture ( "sv_icon_dot0_pix16_gizmo"), //無資料DataPair 灰燈
            EditorGUIUtility.FindTexture ( "sv_icon_dot3_pix16_gizmo"), //監聽 有資料
            EditorGUIUtility.FindTexture ( "Clipboard"), //copy
        };

        public BindingTreeView(TreeViewState state, MultiColumnHeader multicolumnHeader, TreeModel<BindingTreeElement> model) : base(state, multicolumnHeader, model)
        {
            // Custom setup
            rowHeight = kRowHeights;
            columnIndexForTreeFoldouts = 0;
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI	
            Reload();
        }

        // Note we We only build the visible rows, only the backend has the full tree information. 
        // The treeview only creates info for the row list.
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            var rows = base.BuildRows(root);
            return rows;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = (TreeViewItem<BindingTreeElement>)args.item;

            Rect cellRect = args.GetCellRect(0);
            CenterRectUsingSingleLineHeight(ref cellRect);

            base.RowGUI(args);            

            cellRect = args.GetCellRect(1);
            CenterRectUsingSingleLineHeight(ref cellRect);

            if (DataBindingView.OnCopy != null)
            {
                if (GUI.Button(cellRect, s_TestIcons[3]))
                {
                    DataBindingView.OnCopy(item.data.name);
                    DataBindingView.OnCopy = null;
                }
                return;
            }

            if (!EditorApplication.isPlaying)                
                return;

            GUI.Label(cellRect, item.data.DataSource);
            cellRect = args.GetCellRect(2);
            CenterRectUsingSingleLineHeight(ref cellRect);
            //CenterRectUsingSingleLineHeight(ref cellRect);

            if (!item.data.hasSource)
                GUI.DrawTexture(cellRect, s_TestIcons[1], ScaleMode.ScaleToFit);
            else if (item.data.dataPair.IsBindingProperty)
                GUI.DrawTexture(cellRect, s_TestIcons[2], ScaleMode.ScaleToFit);
            else
                GUI.DrawTexture(cellRect, s_TestIcons[0], ScaleMode.ScaleToFit);
            GUI.DrawTexture(args.GetCellRect(3), item.data.hasObserver ? s_TestIcons[2] : s_TestIcons[1], ScaleMode.ScaleToFit);
            GUI.Label(args.GetCellRect(4), item.data.type);
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return true;
        }

        public static MultiColumnHeaderState CreateiColumnHeaderState(float treeViewWidth)
        {
            var columns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Path"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 250,
                    minWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Data"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 500,
                    minWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("pub"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 30,
                    minWidth = 16,
                    autoResize = false,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("sub"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 30,
                    minWidth = 16,
                    autoResize = false,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Type"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 200,
                    minWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = false
                },
            };

            var state = new MultiColumnHeaderState(columns);
            return state;
        }
    }
}
