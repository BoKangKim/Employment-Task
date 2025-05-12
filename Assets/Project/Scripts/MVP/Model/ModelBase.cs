using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.MVP.Model
{
    public interface IModelData
    {
    }

    // 데이터 관리만 하는 클래스
    public abstract class ModelBase
    {
        protected Action<IModelData> onChangedData = null;
        /// <summary>
        /// 초기화
        /// </summary>
        /// <param name="data"> 초기 데이터 </param>
        public abstract void Init(IModelData data = null);

        /// <summary>
        /// Presenter 구독
        /// </summary>
        /// <param name="onChangedData"> 데이터가 변경되었을 때 콜백함수 </param>
        public virtual void Subscribe(Action<IModelData> onChangedData)
        {
            this.onChangedData += onChangedData;
        }

        /// <summary>
        /// 구독해제
        /// </summary>
        /// <param name="onChangedData"> 해제할 콜백 함수 </param>
        public virtual void UnSubcribe(Action<IModelData> onChangedData)
        {
            this.onChangedData -= onChangedData;
        }

        /// <summary>
        /// 현재 데이터 가져오기
        /// </summary>
        /// <returns> 현재 데이터 </returns>
        public abstract IModelData GetData();

        /// <summary>
        /// 데이터 업뎃
        /// </summary>
        /// <param name="data"> 업뎃할 데이터 </param>
        public abstract void UpdateData(IModelData data);
    }
}

