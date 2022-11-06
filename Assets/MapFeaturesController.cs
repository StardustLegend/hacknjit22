using Google.Maps;
using Google.Maps.Event;
using Google.Maps.Feature;
using Google.Maps.Feature.Style;
using Google.Maps.Unity.Intersections;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

namespace TestApp.MapFeatures
{
    public class MapFeaturesController : MonoBehaviour
    {
        [SerializeField] private MapsService _mapsService;
        [Header("Materials")]
        [SerializeField] private Material _intersectionMaterial;
        [SerializeField] private Material _roadMaterial;
        [SerializeField] private Material _roadBorderMaterial;
        [SerializeField] private Material _waterMaterial;
        [SerializeField] private Material _beachMaterial;
        [SerializeField] private Material _grassMaterial;
        [SerializeField] private Material _unspecifiedRegionMaterial;
        [SerializeField] private Material _modeledStructureMaterial;
        [SerializeField] private Material _defaultBuildingWallMaterial;
        [SerializeField] private Material _defaultBuildingRoofMaterial;
        private float _roadWidth = 4;
        [Header("Region Togglers")]
        [SerializeField] private bool _showExtrudedStructures = true;
        [SerializeField] private bool _showModeledStructures = true;
        [SerializeField] private bool _showRegions = true;
        [SerializeField] private bool _showSegments = true;
        [SerializeField] private bool _showAreaWater = true;
        [SerializeField] private bool _showLineWater = true;
        [SerializeField] private bool _showIntersections = true;
        public static GameObjectOptions DefaultGameObjectOptions;
        private RegionStyle _defaultRegionStyle;
        private SegmentStyle _defaultSegmentStyle;
        private AreaWaterStyle _defaultAreaWaterStyle;
        private LineWaterStyle _defaultLineWaterStyle;
        private ModeledStructureStyle _defaultModeledStructureStyle;
        private ExtrudedStructureStyle _defaultExtrudedStructureStyle;
        private void Awake()
        {
            if (_mapsService == null)
                UnityEngine.Debug.Log("Maps Service is required for this script to work.");
            _defaultRegionStyle = new RegionStyle.Builder
            {
                FillMaterial = _grassMaterial,
            }.Build();
            _defaultSegmentStyle = new SegmentStyle.Builder
            {
                Material = _roadMaterial,
                Width = _roadWidth,
                BorderMaterial = _roadBorderMaterial,
                BorderWidth = 0.6f
            }.Build();
            _defaultAreaWaterStyle = new AreaWaterStyle.Builder
            {
                FillMaterial = _waterMaterial
            }.Build();
            _defaultLineWaterStyle = new LineWaterStyle.Builder
            {
                Material = _waterMaterial
            }.Build();
            _defaultModeledStructureStyle = new ModeledStructureStyle.Builder
            {
                Material = _modeledStructureMaterial
            }.Build();
            _defaultExtrudedStructureStyle = new ExtrudedStructureStyle.Builder
            {
                WallMaterial = _defaultBuildingWallMaterial,
                RoofMaterial = _defaultBuildingRoofMaterial
            }.Build();
            DefaultGameObjectOptions = new GameObjectOptions
            {
                ExtrudedStructureStyle = _defaultExtrudedStructureStyle,
                ModeledStructureStyle = _defaultModeledStructureStyle,
                RegionStyle = _defaultRegionStyle,
                AreaWaterStyle = _defaultAreaWaterStyle,
                LineWaterStyle = _defaultLineWaterStyle,
                SegmentStyle = _defaultSegmentStyle,
            };
        }
        private void OnEnable()
        {
            if (_mapsService == null)
                return;
            _mapsService.Events.ExtrudedStructureEvents.WillCreate.AddListener(OnWillCreateExtrudedStructure);
            _mapsService.Events.ModeledStructureEvents.WillCreate.AddListener(OnWillCreateModeledStructure);
            _mapsService.Events.RegionEvents.WillCreate.AddListener(OnWillCreateRegion);
            _mapsService.Events.SegmentEvents.WillCreate.AddListener(OnWillCreateSegment);
            _mapsService.Events.AreaWaterEvents.WillCreate.AddListener(OnWillCreateAreaWater);
            _mapsService.Events.LineWaterEvents.WillCreate.AddListener(OnWillCreateLineWater);
            _mapsService.Events.IntersectionEvents.WillCreate.AddListener(OnWillCreateIntersection);
            _mapsService.Events.RegionEvents.DidCreate.AddListener(OnRegionCreated);
            _mapsService.Events.SegmentEvents.DidCreate.AddListener(OnSegmentCreated);
        }
        void OnDisable()
        {
            if (_mapsService == null)
                return;
            _mapsService.Events.ExtrudedStructureEvents.WillCreate.RemoveListener(OnWillCreateExtrudedStructure);
            _mapsService.Events.ModeledStructureEvents.WillCreate.RemoveListener(OnWillCreateModeledStructure);
            _mapsService.Events.RegionEvents.WillCreate.RemoveListener(OnWillCreateRegion);
            _mapsService.Events.SegmentEvents.WillCreate.RemoveListener(OnWillCreateSegment);
            _mapsService.Events.AreaWaterEvents.WillCreate.RemoveListener(OnWillCreateAreaWater);
            _mapsService.Events.LineWaterEvents.WillCreate.RemoveListener(OnWillCreateLineWater);
            _mapsService.Events.IntersectionEvents.WillCreate.RemoveListener(OnWillCreateIntersection);
            _mapsService.Events.RegionEvents.DidCreate.RemoveListener(OnRegionCreated);
            _mapsService.Events.SegmentEvents.DidCreate.RemoveListener(OnSegmentCreated);
        }
        void OnWillCreateExtrudedStructure(WillCreateExtrudedStructureArgs args)
        {
            args.Cancel = !_showExtrudedStructures;
        }
        void OnWillCreateModeledStructure(WillCreateModeledStructureArgs args)
        {
            args.Cancel = !_showModeledStructures;
        }
        void OnWillCreateRegion(WillCreateRegionArgs args)
        {
            if (_showRegions)
            {
                if (args.MapFeature.Metadata.Usage == RegionMetadata.UsageType.Beach)
                {
                    args.Style = new RegionStyle.Builder(args.Style)
                    {
                        FillMaterial = _beachMaterial
                    }.Build();
                }
                else if (args.MapFeature.Metadata.Usage != RegionMetadata.UsageType.Park
                     || args.MapFeature.Metadata.Usage != RegionMetadata.UsageType.Forest)
                    args.Style = new RegionStyle.Builder(args.Style)
                    {
                        FillMaterial = _unspecifiedRegionMaterial
                    }.Build();
            }
            else
                args.Cancel = true;
        }
        void OnWillCreateSegment(WillCreateSegmentArgs args)
        {
            if (_showSegments && IsValidRoad(args.MapFeature))
            {
                if (!IsTraversableRoad(args.MapFeature))
                    args.Style = new SegmentStyle.Builder(args.Style)
                    {
                        Material = _roadMaterial,
                        Width = _roadWidth * 0.5f,
                        BorderMaterial = _roadBorderMaterial,
                        BorderWidth = 0.3f
                    }.Build();
            }
            else
                args.Cancel = true;
        }
        void OnWillCreateAreaWater(WillCreateAreaWaterArgs args)
        {
            args.Cancel = !_showAreaWater;
        }
        void OnWillCreateLineWater(WillCreateLineWaterArgs args)
        {
            args.Cancel = !_showLineWater;
        }
        void OnWillCreateIntersection(WillCreateIntersectionArgs args)
        {
            if (_showIntersections)
            {
                args.Style = new SegmentStyle.Builder(args.Style)
                {
                    IntersectionMaterial = _intersectionMaterial,
                    IntersectionArmLength = _roadWidth,
                    IntersectionJoinLength = _roadWidth * 4,
                    MaxIntersectionArmDistance = _roadWidth * 5,
                    Width = _roadWidth
                }.Build();
                List<IntersectionArm> arms = new List<IntersectionArm>(args.MapFeature.Shape.Arms);
                foreach (IntersectionArm arm in arms)
                {
                    if (!IsTraversableRoad(arm.Segment))
                    {
                        arm.Cancel = true;
                        continue;
                    }
                }
            }
            else
                args.Cancel = true;
        }
        void OnRegionCreated(DidCreateRegionArgs args)
        {
            if (args.MapFeature.Metadata.Usage == RegionMetadata.UsageType.Park
             || args.MapFeature.Metadata.Usage == RegionMetadata.UsageType.Forest)
                args.GameObject.tag = "Park";
        }
        void OnSegmentCreated(DidCreateSegmentArgs args)
        {
            if (args.MapFeature.Metadata.Usage != SegmentMetadata.UsageType.Highway
             || args.MapFeature.Metadata.Usage != SegmentMetadata.UsageType.ControlledAccessHighway
             || args.MapFeature.Metadata.Usage != SegmentMetadata.UsageType.Rail)
                args.GameObject.tag = "Road";
        }
        public static bool IsTraversableRoad(Segment segment)
        {
            return segment.Metadata.Usage != SegmentMetadata.UsageType.Ferry
                && segment.Metadata.Usage != SegmentMetadata.UsageType.Footpath
                && segment.Metadata.Usage != SegmentMetadata.UsageType.Rail;
        }
        public static bool IsValidRoad(Segment segment)
        {
            return segment.Metadata.Usage != SegmentMetadata.UsageType.Ferry;
        }
    }
}
