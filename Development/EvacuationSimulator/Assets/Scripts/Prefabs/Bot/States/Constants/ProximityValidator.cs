using UnityEngine;

namespace Assets.Scripts.Prefabs.Bot.States.Constants
{
    class ProximityValidator
    {
        private Vector3 _frontCornerRight;
        private Vector3 _frontCornerLeft;
        private Vector3 _backCornerRight;
        private Vector3 _backCornerLeft;

        private float _area;

        private readonly float _lowWidth = 0.3f;
        private readonly float _highWidth = 0.8f;
        private readonly float _length = 2f;

        private Vector3 _oldDirection = Vector3.zero;
        private Vector3 _lastCenter = Vector3.zero;

        BotController _bot;
        private int _botLayerMask = ~(1 << 10);
        private Vector3 _raysStartingPoint;
        private float _raysColliderDistanceToPoints;

        public ProximityValidator(BotController bot)
        {
            _bot = bot;
            _raysStartingPoint = new Vector3(0, 0.86f, 0);
            _raysColliderDistanceToPoints = 1.83f / 2 - 0.25f / 2;
        }

        public bool IsInProximity(Vector3 point, Vector3 rectCenter, Vector3 direction, float distance)
        {
            var angleToCompare = Vector3.SignedAngle(_oldDirection, direction, Vector3.up);

            if (_lastCenter != rectCenter)
            {
                RectangleCorners(rectCenter, direction, angleToCompare);
            }

            var apd = TriangleAreaXZ(point, _frontCornerLeft, _backCornerLeft);
            var dpc = TriangleAreaXZ(point, _backCornerRight, _backCornerLeft);
            var cpb = TriangleAreaXZ(point, _frontCornerRight, _backCornerRight);
            var pba = TriangleAreaXZ(point, _frontCornerLeft, _frontCornerRight);

            //Debug.Log($"sum={apd + dpc + cpb + pba} vs {Vector3.Distance(a, b) * Vector3.Distance(b, c)}");
            if (apd + dpc + cpb + pba > _area)
            {
                return false;
            }
            else
            {
                if ((int)angleToCompare != 0)
                {
                    var bottomPoint = point + _raysStartingPoint - Vector3.up * (_raysColliderDistanceToPoints);
                    var anyObstacle = false;

                    _bot.Dispatcher.Schedule(() =>
                    {
                        anyObstacle = Physics.Raycast(bottomPoint, direction, distance, _botLayerMask);
                    }).WaitOne();

                    if (anyObstacle)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private void RectangleCorners(Vector3 center, Vector3 direction, float angleToCompare)
        {
            direction = direction.normalized;

            var width = _highWidth;
            var angleWidth = 0f;
            //float s = 1;
            //Color color = Color.red;

            if (_oldDirection != Vector3.zero)
            {
                if ((int)Mathf.Abs(angleToCompare) == 90)
                {
                    width = _lowWidth;
                    angleWidth = (-1f * angleToCompare) / 2f;
                    //s = 5;
                    //color = Color.blue;
                    //Debug.Log($"ang old-new : {angleToCompare}");
                }
            }

            direction = Quaternion.AngleAxis(angleWidth, Vector3.up) * direction;

            var mfront = center + direction * width;
            var mback = center - direction * width;

            _frontCornerLeft = mfront + Quaternion.AngleAxis(-90, Vector3.up) * direction * _length;
            _frontCornerRight = mfront + Quaternion.AngleAxis(90, Vector3.up) * direction * _length;
            _backCornerLeft = mback + Quaternion.AngleAxis(-90, Vector3.up) * direction * _length;
            _backCornerRight = mback + Quaternion.AngleAxis(90, Vector3.up) * direction * _length;

            _area = Vector3.Distance(_frontCornerLeft, _frontCornerRight) * Vector3.Distance(_frontCornerRight, _backCornerRight);

            _lastCenter = center;
            _oldDirection = direction;

            //_bot.Dispatcher.Schedule(() =>
            //{
            //    UnityEngine.Debug.DrawLine(_frontCornerLeft, _frontCornerRight, color, s);
            //    UnityEngine.Debug.DrawLine(_frontCornerRight, _backCornerRight, color, s);
            //    UnityEngine.Debug.DrawLine(_backCornerRight, _backCornerLeft, color, s);
            //    UnityEngine.Debug.DrawLine(_backCornerLeft, _frontCornerLeft, color, s);
            //});
        }

        private float TriangleAreaXZ(Vector3 a, Vector3 b, Vector3 c)
        {
            var ab = Mathf.Sqrt(Mathf.Pow((a.x - b.x), 2) + Mathf.Pow((a.z - b.z), 2));
            var bc = Mathf.Sqrt(Mathf.Pow((b.x - c.x), 2) + Mathf.Pow((b.z - c.z), 2));
            var ca = Mathf.Sqrt(Mathf.Pow((c.x - a.x), 2) + Mathf.Pow((c.z - a.z), 2));
            //return Mathf.Abs(a.x*(b.z - c.z) + b.x*(c.z - a.z) + c.x*(a.z - b.z)) / 2;
            var s = (ab + bc + ca) / 2;
            return Mathf.Sqrt(s * (s - ab) * (s - bc) * (s - ca));
        }
    }
}
