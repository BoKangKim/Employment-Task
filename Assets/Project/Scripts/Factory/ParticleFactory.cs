using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Factory
{
    public enum ParticleType
    {
        NONE,
        BlockDestroy
    }

    [Serializable]
    public class ParticleInfo
    {
        [SerializeField] private ParticleType type;
        [SerializeField] private ParticleSystem particle;

        public ParticleType Type => type;
        public ParticleSystem Particle => particle;
    }

    public class ParticleFactory : MonoBehaviour, IFactory
    {
        [SerializeField] private List<ParticleInfo> particleList;

        public ParticleSystem CreateParticle(Vector3 position, Quaternion rotation, ParticleType type ,Material material = null)
        {
            var targetInfo = particleList.Find((value) => value.Type == type);
            ParticleSystem particle = null;

            if(targetInfo != null)
            {
                particle = Instantiate(targetInfo.Particle, position, rotation);
            }

            if(particle != null && material != null)
            {
                ParticleSystem[] pss = particle.GetComponentsInChildren<ParticleSystem>();

                foreach(var ps in pss)
                {
                    ParticleSystemRenderer psrs = ps.GetComponent<ParticleSystemRenderer>();
                    psrs.material = material;
                }
            }

            return particle;
        }
    }
}