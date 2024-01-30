﻿using Player.Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace LowLevel
{
    public class FollowDOTSCameraTarget : MonoBehaviour
    {
        private EntityManager _entityManager;
        private EntityQuery _cameraTargetTagQuery;
        private Transform _transform = null;

        private void Awake()
        {
            _transform = GetComponent<Transform>();

            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _cameraTargetTagQuery = _entityManager.CreateEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(PlayerTag)
                }
            });
        }

        void LateUpdate()
        {
            Entity cameraTargetEntity = _cameraTargetTagQuery.GetSingletonEntity();

            if (_entityManager.HasComponent<LocalToWorld>(cameraTargetEntity))
            {
                LocalToWorld cameraLocalToWorld = _entityManager.GetComponentData<LocalToWorld>(cameraTargetEntity);
                _transform.position = cameraLocalToWorld.Position;
            }
        }
    }
}