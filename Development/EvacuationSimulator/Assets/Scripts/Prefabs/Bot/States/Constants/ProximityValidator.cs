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

        private readonly float _lowWidth = 0.5f;
        private readonly float _highWidth = 0.8f;
        private readonly float _length = 2f;

        private Vector3 _oldDirection = Vector3.zero;
        private Vector3 _lastCenter = Vector3.zero;

        BotController _bot;
        //private int _botLayerMask = ~(1 << 10);
        private LayerMask _rectLayerMask = ~(LayerMask.GetMask("Fire") | LayerMask.GetMask("Bot"));

        public ProximityValidator(BotController bot)
        {
            _bot = bot;
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

            //_bot.Dispatcher.Schedule(() =>
            //{
            //    UnityEngine.Debug.DrawLine(_frontCornerLeft, _frontCornerRight, Color.blue, 0.3f);
            //    UnityEngine.Debug.DrawLine(_frontCornerRight, _backCornerRight, Color.red, 0.3f);
            //    UnityEngine.Debug.DrawLine(_backCornerRight, _backCornerLeft, Color.yellow, 0.3f);
            //    UnityEngine.Debug.DrawLine(_backCornerLeft, _frontCornerLeft, Color.red, 0.3f);
            //});

            if (apd + dpc + cpb + pba > _area)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void RectangleCorners(Vector3 center, Vector3 direction, float angleToCompare)
        {
            var directionToUse = direction.normalized;

            var width = _highWidth;
            var angleWidth = 0f;

            float leftDistance = 0;
            float rightDistance = 0;
            bool hitLeft = false;
            bool hitRight = false;

            if (_oldDirection != Vector3.zero)
            {
                if ((int)Mathf.Abs(angleToCompare) == 90)
                {
                    angleWidth = (-1f * angleToCompare) / 2f;

                    // Update direction
                    directionToUse = Quaternion.AngleAxis(angleWidth, Vector3.up) * directionToUse;

                    _bot.Dispatcher.Schedule(() =>
                    {
                        switch ((int)angleWidth)
                        {
                            case 45:
                                hitLeft = Physics.Raycast(origin: center, direction: Quaternion.AngleAxis(-90, Vector3.up) * directionToUse, hitInfo: out var hitMiddleLeft, maxDistance: 0.75f, layerMask: _rectLayerMask);
                                leftDistance = hitMiddleLeft.distance;
                                break;
                            case -45:
                                hitRight = Physics.Raycast(origin: center, direction: Quaternion.AngleAxis(90, Vector3.up) * directionToUse, hitInfo: out var hitMiddleRight, maxDistance: 0.75f, layerMask: _rectLayerMask);
                                rightDistance = hitMiddleRight.distance;
                                break;
                        }
                    }).WaitOne();
                }
            }


            if (hitLeft || hitRight)
                width = _lowWidth;

            var mfront = center + directionToUse * width;
            var mback = center - directionToUse * width;

            var directionLeft = Quaternion.AngleAxis(-90, Vector3.up) * directionToUse;
            var directionRight = Quaternion.AngleAxis(90, Vector3.up) * directionToUse;

            float distanceFrontLeft = 0;
            float distanceFrontRight = 0;
            float distanceBackLeft = 0;
            float distanceBackRight = 0;

            _bot.Dispatcher.Schedule(() =>
            {
                if (!hitLeft)
                {
                    Physics.Raycast(origin: mfront, direction: directionLeft, hitInfo: out var hitFrontLeft, maxDistance: _length, layerMask: _rectLayerMask);
                    distanceFrontLeft = hitFrontLeft.distance;
                    Physics.Raycast(origin: mback, direction: directionLeft, hitInfo: out var hitBackLeft, maxDistance: _length, layerMask: _rectLayerMask);
                    distanceBackLeft = hitBackLeft.distance;
                }

                if (!hitRight)
                {
                    Physics.Raycast(origin: mfront, direction: directionRight, hitInfo: out var hitFrontRight, maxDistance: _length, layerMask: _rectLayerMask);
                    distanceFrontRight = hitFrontRight.distance;
                    Physics.Raycast(origin: mback, direction: directionRight, hitInfo: out var hitBackRight, maxDistance: _length, layerMask: _rectLayerMask);
                    distanceBackRight = hitBackRight.distance;
                }
            }).WaitOne();

            distanceFrontLeft = distanceFrontLeft == 0 ? _length : distanceFrontLeft;
            distanceFrontRight = distanceFrontRight == 0 ? _length : distanceFrontRight;
            distanceBackLeft = distanceBackLeft == 0 ? _length : distanceBackLeft;
            distanceBackRight = distanceBackRight == 0 ? _length : distanceBackRight;

            if (leftDistance == 0)
                leftDistance = distanceBackLeft < distanceFrontLeft ? distanceBackLeft : distanceFrontLeft;

            if (rightDistance == 0)
                rightDistance = distanceBackRight < distanceFrontRight ? distanceBackRight : distanceFrontRight;

            if(leftDistance < 0.5 && rightDistance < 0.5)
            {
                leftDistance = 2f;
                rightDistance = 2f;
            }

            _frontCornerLeft = mfront + directionLeft * leftDistance;
            _frontCornerRight = mfront + directionRight * rightDistance;
            _backCornerLeft = mback + directionLeft * leftDistance;
            _backCornerRight = mback + directionRight * rightDistance;

            _area = Vector3.Distance(_frontCornerLeft, _frontCornerRight) * Vector3.Distance(_frontCornerRight, _backCornerRight);

            _lastCenter = center;
            _oldDirection = direction.normalized;
        }

        private float TriangleAreaXZ(Vector3 a, Vector3 b, Vector3 c)
        {
            var ab = Mathf.Sqrt(Mathf.Pow((a.x - b.x), 2) + Mathf.Pow((a.z - b.z), 2));
            var bc = Mathf.Sqrt(Mathf.Pow((b.x - c.x), 2) + Mathf.Pow((b.z - c.z), 2));
            var ca = Mathf.Sqrt(Mathf.Pow((c.x - a.x), 2) + Mathf.Pow((c.z - a.z), 2));
            var s = (ab + bc + ca) / 2;
            return Mathf.Sqrt(s * (s - ab) * (s - bc) * (s - ca));
        }
    }
}
