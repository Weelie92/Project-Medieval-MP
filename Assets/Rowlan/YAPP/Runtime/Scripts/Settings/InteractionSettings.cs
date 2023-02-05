using UnityEngine;

namespace Rowlan.Yapp
{
    [System.Serializable]
    public class InteractionSettings
    {
        public enum InteractionType
        {
            AntiGravity,
            Magnet,
            ChangeScale,
            SetScale
        }

        [System.Serializable]
        public class AntiGravity
        {
            /// <summary>
            /// Anti Gravity strength from 0..100
            /// </summary>
            [Range(0, 100)]
            public int strength = 30;

        }

        [System.Serializable]
        public class Magnet
        {
            /// <summary>
            /// Some arbitrary magnet strength from 0..100
            /// </summary>
            [Range(0, 100)]
            public int strength = 10;

        }

        [System.Serializable]
        public class ChangeScale
        {
            /// <summary>
            /// Some arbitrary strength from 0..100
            /// </summary>
            [Range(0, 100)]
            public float changeScaleStrength = 10f;

        }

        [System.Serializable]
        public class SetScale
        {
            /// <summary>
            /// Some arbitrary strength from 0..10
            /// </summary>
            [Range(0, 10)]
            public float setScaleValue = 1f;

        }

        #region Public Editor Fields

        public InteractionType interactionType;

        public AntiGravity antiGravity = new AntiGravity();
        public Magnet magnet = new Magnet();
        public ChangeScale changeScale = new ChangeScale();
        public SetScale setScale = new SetScale();

        #endregion Public Editor Fields



    }
}
