using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attribute;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control 
{
    public class PlayerController : MonoBehaviour
    {
        Health health;



        [Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;

        }

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float navMeshProjectionDistance = 1f;
        [SerializeField] float raycastRadius = 1f;
         

        private void Awake()
        {
            health = GetComponent<Health>();
        }

        void Update()
        {
            if (InteractWithUI()) return;

            if (health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }

            if (InteractWithComponent()) return;
            //if(InteractWithCombat()) return;
            if(InteractWithMovement()) return;

            SetCursor(CursorType.None);

        }

        private bool InteractWithComponent()
        {

            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables =  hit.transform.GetComponents<IRaycastable>();
                foreach(IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        //将hit根据与Player的距离排序
        private RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            float[] distances = new float[hits.Length];
            for(int i =0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            //根据distance将hits排序
            Array.Sort(distances, hits);

            return hits;
        }

        //整合到InteractWithComponent()中
        //private bool InteractWithCombat()
        //{
        //    RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
        //    foreach(RaycastHit hit in hits)
        //    {
        //        if (Input.GetMouseButtonDown(0))
        //        {



        //        }
        //        SetCursor(CursorType.Combat);

        //    }
        //    return false;
        //}

        //InteractWithMovement与navMeshAgent互动，无法添加IRaycastable，故不作整合
        private bool InteractWithMovement()
        {
            //RaycastHit hit;
            //bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);

            if (hasHit)
            {

                if (!GetComponent<Mover>().CanMoveTo(target)) return false;

                if (Input.GetMouseButton(0))
                {

                    GetComponent<Mover>().StartMoveAction(target, 1f);


                }
                SetCursor(CursorType.Movement);
                return true;
            }

            return false;
        }

        //点击到不能移动位置显示不能移动Cursor，移动到可以移动到的,raycastHit.point的最近距离
        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) return false;

            NavMeshHit navMeshHit;
            bool hasNav = NavMesh.SamplePosition(hit.point,out navMeshHit, navMeshProjectionDistance, NavMesh.AllAreas );
            if (!hasNav) return false;

            target = navMeshHit.position;



            return true;
        }

        private bool InteractWithUI()
        {
            if (EventSystem.current == null) return false;
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false; 
        }


        static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        CursorMapping GetCursorMapping(CursorType type)
        {

           foreach(CursorMapping mapping in cursorMappings)
            {
                if(mapping.type == type)
                {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }



    }
}



