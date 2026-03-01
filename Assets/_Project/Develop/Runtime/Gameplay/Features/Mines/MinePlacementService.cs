using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Mines;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Utilites;
using Assets._Project.Develop.Runtime.Utilites.AssetsManagment;
using Assets._Project.Develop.Runtime.Utilites.RaycastManagment;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Mines
{
    public class MinePlacementService
    {
        private readonly MainHeroesHolderService _mainHeroesHolder;
        private readonly SurfaceRaycaster _raycaster;
        private readonly Camera _camera;
        private readonly WaterMineConfig _config;
        private readonly ResourcesAssetsLoader _resources;

        private GameObject _ghost;
        private Renderer[] _ghostRenderers;
        private bool _isPlacing;

        private static readonly Color ValidColor = new Color(0f, 1f, 0f, 0.5f);
        private static readonly Color InvalidColor = new Color(1f, 0f, 0f, 0.5f);

        public bool IsPlacing => _isPlacing;

        public MinePlacementService(
            MainHeroesHolderService mainHeroesHolder,
            SurfaceRaycaster raycaster,
            Camera camera,
            WaterMineConfig config,
            ResourcesAssetsLoader resources)
        {
            _mainHeroesHolder = mainHeroesHolder;
            _raycaster = raycaster;
            _camera = camera;
            _config = config;
            _resources = resources;
        }

        public void BeginPlacement()
        {
            if (_isPlacing)
                return;

            Debug.Log("Begin placement!");

            GameObject prefab = _resources.Load<GameObject>(_config.GhostPrefabPath);
            _ghost = Object.Instantiate(prefab);
            _ghostRenderers = _ghost.GetComponentsInChildren<Renderer>();
            _isPlacing = true;
        }

        public void Update(float deltaTime)
        {
            if (!_isPlacing)
                return;

            if (!_raycaster.TryGetHitInfo(_camera, LayersAPI.LayerMaskWater, out RaycastHit hit))
                return;

            Vector3 position = hit.point;
            _ghost.transform.position = position;

            bool isValid = IsPositionValid(position);
            SetGhostColor(isValid ? ValidColor : InvalidColor);

            if (Input.GetMouseButtonDown(0))
            {
                if (isValid)
                    PlaceMine(position);
                else
                    CancelPlacement();
            }
        }

        public void CancelPlacement()
        {
            if (!_isPlacing)
                return;

            Object.Destroy(_ghost);
            _ghost = null;
            _isPlacing = false;
        }

        private void PlaceMine(Vector3 position)
        {
            // TODO: создать Entity мины через фабрику
            Object.Destroy(_ghost);
            _ghost = null;
            _isPlacing = false;
        }

        private bool IsPositionValid(Vector3 position)
        {
            if (_mainHeroesHolder.MainShip == null)
                return false;

            Vector3 shipPosition = _mainHeroesHolder.MainShip.Transform.position;
            float distanceToShip = Vector3.Distance(position, shipPosition);

            if (distanceToShip > _config.MaxPlacementDistance)
                return false;

            Collider[] overlaps = Physics.OverlapSphere(position, _config.OverlapCheckRadius, LayersAPI.LayerMaskHittable);
            return overlaps.Length == 0;
        }

        private void SetGhostColor(Color color)
        {
            foreach (Renderer r in _ghostRenderers)
            {
                foreach (Material mat in r.materials)
                    mat.color = color;
            }
        }
    }
}