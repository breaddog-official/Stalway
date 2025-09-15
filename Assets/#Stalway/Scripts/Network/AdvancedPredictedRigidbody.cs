using Mirror;
using UnityEngine;

namespace Breaddog.Network
{
    public class AdvancedPredictedRigidbody : PredictedRigidbody
    {
        [Space]
        public double timeForSync = 2d;

        protected double? lastMove;

        protected override void Awake()
        {
            lastMove = NetworkTime.time;

            base.Awake();
        }

        protected override void OnBeginPrediction()
        {
            lastMove = null;
        }

        protected override void OnEndPrediction()
        {
            lastMove = NetworkTime.time;
        }


        private void Update()
        {
            if (syncPosition && isClientOnly && isOwned)
            {
                double threshold = lastMove.HasValue ? Lerp(positionCorrectionThreshold, 0d, (NetworkTime.time - lastMove.Value) / timeForSync) : positionCorrectionThreshold;
                positionCorrectionThresholdSqr = threshold * threshold;

                if (!reduceSendsWhileIdle) // �� ������ ��������� reduceSendsWhileIdle ���� ������� ��������
                    syncInterval = threshold <= 0.01d ? 1 : 0;
            }
        }

        private static double Lerp(double a, double b, double t)
        {
            return a + (b - a) * Clamp01(t);
        }

        private static double Clamp01(double value)
        {
            if (value < 0d)
            {
                return 0d;
            }

            if (value > 1d)
            {
                return 1d;
            }

            return value;
        }
    }
}