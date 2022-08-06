using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

namespace rc_hololens
{
    public class ReadFbx : MonoBehaviour
    {
        private PanelBigHierarchyScript panelBigHierarchyScript;

        private Quaternion rotateZ90;

        private RectTransform panelBigHierarchyRectTrans;

        private void Awake()
        {
            panelBigHierarchyScript = GameObject.Find("PanelBigHierarchy").GetComponent<PanelBigHierarchyScript>();
            rotateZ90.x = 0f;
            rotateZ90.y = 0f;
            rotateZ90.z = 1f / Mathf.Sqrt(2f);
            rotateZ90.w = rotateZ90.z;
            panelBigHierarchyRectTrans = panelBigHierarchyScript.transform as RectTransform;
        }

        public void LoadFbxFile(string filename)
        {
            if (panelBigHierarchyScript.markers.Count == 0)
            {
                panelBigHierarchyScript.CreateMaker();
            }
            var gameObject = FbxLoaderUtils.LoadFile(filename);
            gameObject.name = Path.GetFileNameWithoutExtension(filename);
            SceneItemScript sceneItemScript = gameObject.AddComponent<SceneItemScript>();
            sceneItemScript.counterButtonImageRectTrans.rotation = rotateZ90;
            sceneItemScript.imageCom.color = Color.white;
            sceneItemScript.spritePos = 1;
            sceneItemScript.counterButtonTextRectTrans.GetChild(0).GetComponent<Text>().text = gameObject.name;
            GameObject gameObject2 = panelBigHierarchyScript.markers[panelBigHierarchyScript.markers.Count - 1];
            gameObject2.GetComponent<SceneItemScript>().imageCom.color = Color.white;
            gameObject2.GetComponent<SceneItemScript>().drawChildren = true;
            gameObject.transform.SetParent(gameObject2.transform);
            gameObject.SetActive(true);

            AddScreenItemScript(gameObject);
            
            panelBigHierarchyScript.RedrawSceneItemButton();
        }

        private void AddScreenItemScript(GameObject gameObject)
        {
            var childrenCount = gameObject.transform.childCount;
            for (var index = 0; index < childrenCount; ++index)
            {
                var childTransform = gameObject.transform.GetChild(index);
                var child = childTransform.gameObject;

                var screenItem = child.AddComponent<SceneItemScript>();
                screenItem.spritePos = 2;
                screenItem.imageCom.color = Color.clear;
                RectTransform counterButtonImageRectTrans = screenItem.counterButtonImageRectTrans;
                counterButtonImageRectTrans.offsetMin = panelBigHierarchyScript.sceneItemSpriteOffsetMin;
                counterButtonImageRectTrans.offsetMax = panelBigHierarchyScript.sceneItemSpriteOffsetMin + panelBigHierarchyScript.sceneItemSpriteOffsetMinMax;
                RectTransform counterButtonTextRectTrans = screenItem.counterButtonTextRectTrans;
                counterButtonTextRectTrans.offsetMin = panelBigHierarchyScript.sceneItemSpriteOffsetMin * 2f;
                counterButtonTextRectTrans.offsetMax = panelBigHierarchyScript.sceneItemTextOffsetMax;
                counterButtonTextRectTrans.GetChild(0).GetComponent<Text>().text = child.name;
                screenItem.HideCanvasGroup();
                
                AddScreenItemScript(child);
            }
        }
        
    }
}