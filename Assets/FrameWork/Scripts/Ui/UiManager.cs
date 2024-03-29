using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FrameWork
{
    public class UiManager : SingletonAsClass<UiManager>
    {
        
        private Transform CanvasTransform = null;

        private Transform BackgroundTransform = null;
        
        private Transform NormalTransform = null;
        
        private Transform PopupTransform = null;
        
        private Transform ControlTransform = null;

        private int _index;
        
        
        private Stack<Actor> _uiStack;

        private UiRoot _uiRoot;
        public void Init()
        {
            _index = 0;
            //_uiStack.Clear();
            ClearAllPanel();
        }

        public UiManager()
        {
            _uiStack = new Stack<Actor>();
            //var prefab = AssetBundlesLoad.LoadAsset<GameObject>("Ui", "UiRoot");
            //CanvasTransform= GameObject.Instantiate(prefab)?.transform;

            _uiRoot = new UiRoot();
            CanvasTransform = _uiRoot.GetGameObject().transform;
            if (CanvasTransform!=null)
            {
                BackgroundTransform =CanvasTransform.Find("Background");
                NormalTransform =CanvasTransform.Find("Normal");
                PopupTransform =CanvasTransform.Find("Popup");
                ControlTransform =CanvasTransform.Find("Control");
            }
        }


        public void ShowUi(int index)
        {
            EventManager.DispatchEvent(MessageType.UiMessage,UiMessageType.Show,new object[]{index});
            //ShowUiAction?.Invoke(index);
        }
        
        public UiActor ShowUi(string type)
        {
            var t = Assembly.GetExecutingAssembly().GetType(type);
            return ShowUi(t);
        }

        
        private UiActor ShowUi(Type type)
        {
            if (CanvasTransform==null)
            {
                MyLog.LogError("场景中没有Canvas组件,无法显示Ui物体");
                return null;
            }
            
            Type t = type;
            string fullName = t.Name;
            var uiMode=t.GetCustomAttribute<UiModeAttribute>();

            if (uiMode==null)
            {
                MyLog.LogError("类不具备UiModeAttribute");
                return null;
            }
           
            Transform tran=GetTransform(uiMode);

            var param = new object[] { tran };
            UiActor obj;
            try
            {
                obj= (UiActor)Activator.CreateInstance(t, param);
            }
            catch (Exception e)
            {
                
                MyLog.LogError(e.Message);
                return null;
            }
            
            //var obj =(T)Assembly.GetExecutingAssembly().CreateInstance(t.Namespace+"."+fullName)
            if (obj==null)
            {
                MyLog.LogError("生成ui失败");
                return null;
            }
            
            obj.SetIndex(_index);
            _index += 1;
            _uiStack.Push(obj);
            return obj;
        }
        

        public UiActor ShowUi<T>() where T: UiActor
        {

            if (CanvasTransform==null)
            {
                MyLog.LogError("场景中没有Canvas组件,无法显示Ui物体");
                return null;
            }
            
            Type t = typeof(T);
            string fullName = t.Name;
            var uiMode=t.GetCustomAttribute<UiModeAttribute>();

            if (uiMode==null)
            {
                MyLog.LogError("类不具备UiModeAttribute");
                return null;
            }
           
            Transform tran=GetTransform(uiMode);

            var param = new object[] { tran };
            var obj=(T)Activator.CreateInstance(t,param);
            
            //var obj =(T)Assembly.GetExecutingAssembly().CreateInstance(t.Namespace+"."+fullName)
            if (obj==null)
            {
                MyLog.LogError("生成ui失败");
                return null;
            }
            
            obj.SetIndex(_index);
            _index += 1;
            _uiStack.Push(obj);
            return obj;
        }
        public void HideUi(int index)
        {
            EventManager.DispatchEvent(MessageType.UiMessage,UiMessageType.Hide,new object[]{index});
            //HideUiAction?.Invoke(index);
        }

        
        public void RemoveUi(int index)
        {
            EventManager.DispatchEvent(MessageType.UiMessage,UiMessageType.Remove,new object[]{index});
            //RemoveUiAction?.Invoke(index);
        }


        public void Back()
        {
            if (_uiStack.Count>0)
            {
                var actor=_uiStack.Pop();
                if (actor!=null)
                {
                    RemoveUi(actor.GetIndex());
                }
                else
                {
                    Back();
                }
            }
        }
        
        public void ClearAllPanel()
        {
            RemoveUi(-1);
            //RemoveUiAction?.Invoke(-1);
            _uiStack.Clear();
        }


        public Transform GetTransform(UiModeAttribute uiModeAttribute)
        {
            Transform tran = null;
            switch (uiModeAttribute.Mode)
            {
                case Mode.Background:
                    tran = BackgroundTransform;
                    break;
                case Mode.Normal:
                    tran = NormalTransform;
                    break;
                case Mode.Popup:
                    tran = PopupTransform;
                    break;
                case Mode.Control:
                    tran = ControlTransform;
                    break;
                
            }

            return tran;
        }
        

    }
}