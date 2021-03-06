﻿using UnityEngine;

namespace Seiro.ParticleSystem2D.Core.RefDatas
{

    /// <summary>
    /// エッジの物理設定と描画設定
    /// </summary>
    [System.Serializable]
    public class RefEdge
    {

        public uint uid;

        public uint aUID;
        public uint bUID;
        public float width;
        public Color color;
        public string materialID;

        public RefEdge(uint uid, uint aUID, uint bUID, float width, Color color, string materialID)
        {
            this.uid = uid;
            this.aUID = aUID;
            this.bUID = bUID;
            this.width = width;
            this.color = color;
            this.materialID = materialID;
        }

        /// <summary>
        /// 指定したUIDを含んでいるか確認する
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool ContainsParticle(uint uid)
        {
            return aUID == uid || bUID == uid;
        }
    }
}