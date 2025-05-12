using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.Events;

namespace Game.MVP.View
{
    public interface IEventData
    {

    }

    // Viewing을 담당하는 클래스
    public abstract class ViewBase : MonoBehaviour
    {
        protected virtual void Awake()
        {
            Init();
        }

        protected Action<IEventData> eventData = null;

        /// <summary>
        /// 초기화 함수 이 함수가 호출될 때 Presenter랑 Connect 해주는 것이 좋음
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// 데이터가 변경되었을 때 Presenter에서 호출해주는 함수 데이터 변경을 토대로 View를 업데이트
        /// </summary>
        /// <param name="data"> 변경된 데이터 </param>
        public abstract void UpdateView(Model.IModelData data);

        /// <summary>
        /// Presenter가 Observing할 수 있도록 구독
        /// </summary>
        /// <param name="eventData"> 이벤트 발생 시 Presenter에게 알림 </param>
        public abstract void Subscribe(Action<IEventData> eventData);

        /// <summary>
        /// 구독 해제, 씬이 변경될 때나 오브젝트가 파괴될 때 구독해줘야 함
        /// 안해주면 메모리 누수 가능성 존재
        /// </summary>
        public abstract void UnSubcribe();
    }
}