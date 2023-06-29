using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomCollisionDetection
{
    public abstract class AreaManager : MonoBehaviour
    {
        [Header ("Custom Bound Setting")]
        [SerializeField]
        private bool enableDetection = true;

        [SerializeField]
        protected Transform target = null;

        [SerializeField]
        private bool autoSetTargetPlayer = true;

        [SerializeField]
        private bool useOnceDetection = true;

        [SerializeField]
        protected bool useAutoCallBack = true;

        [SerializeField]
        protected Color color = Color.blue;

        [SerializeField]
        private Color disabledColor = Color.gray;

        [SerializeField]
        private Vector3 boundScale = Vector3.one;

        [SerializeField]
        private Vector3 center = Vector3.zero;

        private object defaultCallbackType = null;

        protected AreaBound area = new AreaBound ();

        private bool detection = false;

        private readonly Vector3 wrongPosition = new Vector3 (-9999f, -9999f, -9999f);

        public event Action<object> eventAction;

        internal Vector3 pos {
            get {
                return transform.position + center;
            }
        }

        internal Vector3 size {
            get {
                return boundScale;
            }
        }

        internal Vector3 TargetPosition {
            get {
                if ( target ) {

                    return target.transform.position;
                }
                return wrongPosition;
            }
        }

        internal Vector3 TargetSizeDelta {
            get {
                if ( target ) {
                    Collider col = target.GetComponent<Collider> ();
                    Renderer rend = target.GetComponent<Renderer> ();

                    if ( col || rend ) {
                        Vector3 extend = !col ? rend.bounds.extents : col.bounds.extents;
                        return extend;
                    }
                }
                return Vector3.zero;
            }
        }

        internal Transform Target {
            get {
                return target;
            }
        }

        public virtual void FindTargetPlayer ()
        {
            var findTarget = GameObject.FindGameObjectWithTag ("Player").transform;
            if ( findTarget ) {
                SetTarget (findTarget);
            }
            else {
                Debug.LogWarning ("Target positions not find! Collision Detection not worked! Please setup target.");
                return;
            }
        }

        protected void SetTarget (Transform newTransform)
        {
            target = newTransform;
        }

        private void Start ()
        {
            area = new AreaBound (pos, boundScale);

            if ( autoSetTargetPlayer && target == null ) {
                FindTargetPlayer ();
            }
            Initialize ();
        }

        public virtual void Initialize ()
        {

        }

        public virtual void OnReset ()
        {

        }

        private void Update ()
        {
            if ( (target == null || !target.gameObject.activeSelf) || !enableDetection || (detection && useOnceDetection) ) {
                return;
            }

            if ( area.OverLaps (TargetPosition, TargetSizeDelta) ) {
                OnOverlap (eventAction);

                if ( useAutoCallBack ) {
                    Action<object> action = eventAction;
                    if ( action != null ) {
                        action (defaultCallbackType);
                    }
                }
                if ( detection == false ) {
                    detection = true;
                    if ( useOnceDetection ) {
                        color = disabledColor;
                    }
                }
            }
        }

        private void Reset ()
        {
            area = new AreaBound ();
            target = null;
            detection = false;
            OnReset ();
        }

        protected abstract void OnOverlap (Action<object> action);

        protected virtual void OnDrawGizmos ()
        {
            Gizmos.color = color;
            Gizmos.DrawWireCube (pos, size);
        }
    }
}