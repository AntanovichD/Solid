using System;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidWorksApi
{
    /// <summary>
    /// Опции операции «Вытянуть бобышку/Вырез». Соответствуют параметрам
    /// диалога SolidWorks (Направление 1, тип условия завершения, глубина).
    /// </summary>
    public sealed class ExtrudeOptions
    {
        public swEndConditions_e EndCondition1 { get; set; }
            = swEndConditions_e.swEndCondBlind;

        public swEndConditions_e EndCondition2 { get; set; }
            = swEndConditions_e.swEndCondBlind;

        public double DepthMm1 { get; set; }
        public double DepthMm2 { get; set; }

        public bool BothDirections { get; set; }
        public bool FlipDirection { get; set; }

        public static ExtrudeOptions Blind(double depthMm) =>
            new ExtrudeOptions { DepthMm1 = depthMm };

        public static ExtrudeOptions MidPlane(double depthMm) =>
            new ExtrudeOptions
            {
                EndCondition1 = swEndConditions_e.swEndCondMidPlane,
                DepthMm1 = depthMm
            };

        public static ExtrudeOptions ThroughAll() =>
            new ExtrudeOptions
            {
                EndCondition1 = swEndConditions_e.swEndCondThroughAll
            };
    }

    /// <summary>
    /// Обёртка над <see cref="IFeatureManager"/>: выдавливание и вырез по
    /// открытому/закрытому эскизу, выбор плоскостей и граней по имени.
    /// </summary>
    public sealed class FeatureBuilder
    {
        private readonly SolidWorksHost _host;

        public FeatureBuilder(SolidWorksHost host) => _host = host;

        public IFeature Extrude(ExtrudeOptions o)
        {
            if (o == null) o = new ExtrudeOptions();
            return _host.FeatureManager.FeatureExtrusion3(
                Sd: true,
                Flip: o.FlipDirection,
                Dir: false,
                T1: (int)o.EndCondition1,
                T2: (int)o.EndCondition2,
                D1: Mm.ToMeters(o.DepthMm1),
                D2: Mm.ToMeters(o.DepthMm2),
                Dchk1: false,
                Dchk2: false,
                Ddir1: false,
                Ddir2: false,
                Dang1: 0,
                Dang2: 0,
                OffsetReverse1: false,
                OffsetReverse2: false,
                TranslateSurface1: false,
                TranslateSurface2: false,
                Merge: true,
                UseFeatScope: true,
                UseAutoSelect: true,
                T0: (int)swStartConditions_e.swStartSketchPlane,
                StartOffset: 0,
                FlipStartOffset: false);
        }

        public IFeature Cut(ExtrudeOptions o)
        {
            if (o == null) o = new ExtrudeOptions();
            return _host.FeatureManager.FeatureCut4(
                Sd: true,
                Flip: o.FlipDirection,
                Dir: false,
                T1: (int)o.EndCondition1,
                T2: (int)o.EndCondition2,
                D1: Mm.ToMeters(o.DepthMm1),
                D2: Mm.ToMeters(o.DepthMm2),
                Dchk1: false,
                Dchk2: false,
                Ddir1: false,
                Ddir2: false,
                Dang1: 0,
                Dang2: 0,
                OffsetReverse1: false,
                OffsetReverse2: false,
                TranslateSurface1: false,
                TranslateSurface2: false,
                NormalCut: true,
                UseFeatScope: true,
                UseAutoSelect: true,
                AssemblyFeatureScope: false,
                AutoSelectComponents: false,
                PropagateFeatureToParts: false,
                T0: (int)swStartConditions_e.swStartSketchPlane,
                StartOffset: 0,
                FlipStartOffset: false,
                OptimizeGeometry: true);
        }

        /// <summary>
        /// Выбирает грань построенной фичи по индексу. У объекта Feature
        /// есть GetFaces — возвращает массив граней; индекс определяется
        /// порядком построения.
        /// </summary>
        public bool SelectFaceOfFeature(IFeature feature, int faceIndex)
        {
            if (feature == null) throw new ArgumentNullException(nameof(feature));
            var faces = (object[])feature.GetFaces();
            if (faces == null || faceIndex < 0 || faceIndex >= faces.Length)
                return false;
            var entity = (IEntity)faces[faceIndex];
            return entity.Select4(false, null);
        }
    }
}
