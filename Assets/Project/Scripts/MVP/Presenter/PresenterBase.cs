using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Game.MVP.View;
using Game.MVP.Model;

namespace Game.MVP.Presenter
{
    public abstract class PresenterBase
    {
        protected ModelBase model = null;
        protected ViewBase view = null;

        public bool IsConnectModel => model != null;
        public bool IsConnectView => view != null;

        public bool IsSafeConnect => IsConnectModel && IsConnectView;

        public event Action<IModelData> onChangedData = null;

        /// <summary>
        /// Model 구독
        /// </summary>
        /// <param name="model"> 구독 할 Model </param>
        public void Observe(ModelBase model)
        {
            if (this.model != null)
            {
                Debug.LogWarning($"Already Observing {this.model}, Unsubscribe First");
                return;
            }

            this.model = model;
            this.model.Subscribe(OnChangedModel);

            if (IsConnectView)
            {
                view.UpdateView(this.model.GetData());
            }

            Debug.Log($"[Observing] {model}");
        }

        /// <summary>
        /// View 구독
        /// </summary>
        /// <param name="view"> 구독 할 View</param>
        public void Observe(ViewBase view)
        {
            if (this.view != null)
            {
                Debug.LogWarning($"Already Observing {this.view}, Unsubscribe First");
                return;
            }

            this.view = view;
            this.view.Subscribe(OnEvent);

            if (IsConnectModel)
            {
                this.view.UpdateView(this.model.GetData());
            }

            Debug.Log($"[Observing] {view}");
        }

        /// <summary>
        /// 모델 구독 해제
        /// </summary>
        public void UnObserveModel()
        {
            if (model == null)
            {
                Debug.LogWarning($"Not Observing {this.model}, Subscribe First");
                return;
            }

            this.model.UnSubcribe(OnChangedModel);
            this.model = null;

            Debug.Log($"[UnObserving] {model}");
        }

        /// <summary>
        /// 뷰 구독 해제
        /// </summary>
        public void UnObserveView()
        {
            if (view == null)
            {
                Debug.LogWarning($"Not Observing {this.view}, Subscribe First");
                return;
            }

            this.view.UnSubcribe();
            this.view = null;

            Debug.Log($"[UnObserving] {view}");
        }

        /// <summary>
        /// Data 변경 시 호출되는 콜백 -> View에게 전달하여 View 업데이트
        /// </summary>
        /// <param name="data"> 변경된 데이터 </param>
        protected virtual void OnChangedModel(IModelData data)
        {
            view?.UpdateView(data);
            onChangedData?.Invoke(data);
        }

        /// <summary>
        /// 이벤트 발생 시 호출되는 함수
        /// </summary>
        /// <param name="eventData"> 해당 이벤트에 대한 정보 </param>
        protected abstract void OnEvent(IEventData eventData);

        public IModelData GetData()
        {
            if (model == null)
            {
                return null;
            }

            return model.GetData();
        }

        #region View Event Logic

        #endregion
    }
}

