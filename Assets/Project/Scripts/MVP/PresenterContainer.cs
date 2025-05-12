using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.MVP.Presenter;
using Game.MVP.Model;
using Game.MVP.View;

namespace Game.MVP
{
    public class PresenterContainer
    {
        #region Singleton
        private PresenterContainer()
        {
        }

        private static PresenterContainer instance = null;

        public static PresenterContainer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PresenterContainer();
                }

                return instance;
            }
        }
        #endregion

        private Dictionary<string, PresenterBase> presenterDict = new Dictionary<string, PresenterBase>();

        public void ConnectPresenter<T>(ModelBase model) where T : PresenterBase, new()
        {
            string type = typeof(T).ToString();

            PresenterBase target = null;

            if (presenterDict.TryGetValue(type, out target))
            {
                if (!target.IsConnectModel)
                {
                    target.Observe(model);
                    UnityEngine.Debug.Log($"[Connect] {type} - {model}");
                }
            }
            else
            {
                target = new T();
                target.Observe(model);
                presenterDict.TryAdd(type, target);
                UnityEngine.Debug.Log($"[Connect] {type} - {model}");
            }
        }

        public void ConnectPresenter<T>(ViewBase view) where T : PresenterBase, new()
        {
            string type = typeof(T).ToString();

            PresenterBase target = null;

            if (presenterDict.TryGetValue(type, out target))
            {
                if (!target.IsConnectView)
                {
                    target.Observe(view);
                    UnityEngine.Debug.Log($"[Connect] {type} - {view}");
                }
            }
            else
            {
                target = new T();
                target.Observe(view);
                presenterDict.TryAdd(type, target);
                UnityEngine.Debug.Log($"[Connect] {type} - {view}");
            }
        }

        public void RemovePresenter<T>() where T : PresenterBase
        {
            string type = typeof(T).ToString();

            if (presenterDict.TryGetValue(type, out PresenterBase presenter))
            {
                if (presenter != null)
                {
                    presenter.UnObserveModel();
                    presenter.UnObserveView();
                }

                presenterDict.Remove(type);
            }
        }

        public void Clear()
        {
            foreach (var presenter in presenterDict)
            {
                presenter.Value.UnObserveModel();
                presenter.Value.UnObserveView();
            }

            presenterDict.Clear();
        }

        public T GetPresenter<T>() where T : PresenterBase, new()
        {
            string type = typeof(T).ToString();
            PresenterBase target = null;

            if (!presenterDict.TryGetValue(type, out target))
            {
                target = new T();
                presenterDict.TryAdd(type, target);
            }

            return target as T;
        }
    }
}

