using System;
using UnityEngine;

namespace MonsterHunterMod.Characters.Survivors.Glaive.Content.Misc._Content
{
    internal class GlowBreathe : MonoBehaviour
    {
        public Color color = Color.white;
        private Color lerpedColor;
        public float speed = .4f;
        float time;
        public bool isAnOddVariation = false;
        public enum State
        {
            stopped,
            breathing
        }

        public State state = State.breathing;

        private SpriteRenderer renderer;

        void OnEnable()
        {
            renderer = GetComponent<SpriteRenderer>();
            color = renderer.color;
            lerpedColor = color;
        }

        void OnDisable()
        {
            color = Color.clear;
            lerpedColor = Color.clear;
        }

        public void Update()
        {
            if (!isAnOddVariation)
            {
                if (color != null && renderer != null && state == State.breathing)
                {
                    time = 0;
                    lerpedColor = Color.Lerp(Color.white, Color.clear, Mathf.PingPong(Time.time, speed));
                    renderer.color = lerpedColor;
                }
                if (state == State.stopped)
                {
                    time += Time.time;
                    lerpedColor = Color.white;
                }
            }
            if (isAnOddVariation)
            {
                if (color != null && renderer != null && state == State.breathing)
                {
                    time = 0;
                    lerpedColor = Color.Lerp(color, Color.clear, Mathf.PingPong(Time.time, speed));
                    renderer.color = lerpedColor;
                }
                if (state == State.stopped)
                {
                    time += Time.time;
                    lerpedColor = color;
                }
            }
            
        }
    }
}
