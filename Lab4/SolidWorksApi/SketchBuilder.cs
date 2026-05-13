using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidWorksApi
{
    /// <summary>
    /// Тонкая обёртка над <see cref="ISketchManager"/> и <see cref="IModelDoc2"/>,
    /// принимающая координаты и размеры в миллиметрах. Скрывает преобразование
    /// единиц и порядок аргументов SolidWorks API.
    /// </summary>
    public sealed class SketchBuilder
    {
        private readonly IModelDoc2 _doc;
        private readonly ISketchManager _sm;

        public SketchBuilder(SolidWorksHost host)
        {
            _doc = host.ModelDoc;
            _sm = host.SketchManager;
        }

        public ISketchSegment Line(double x1Mm, double y1Mm, double x2Mm, double y2Mm)
            => _sm.CreateLine(
                Mm.ToMeters(x1Mm), Mm.ToMeters(y1Mm), 0,
                Mm.ToMeters(x2Mm), Mm.ToMeters(y2Mm), 0);

        public ISketchSegment CenterLine(double x1Mm, double y1Mm, double x2Mm, double y2Mm)
            => _sm.CreateCenterLine(
                Mm.ToMeters(x1Mm), Mm.ToMeters(y1Mm), 0,
                Mm.ToMeters(x2Mm), Mm.ToMeters(y2Mm), 0);

        public ISketchSegment Circle(double cxMm, double cyMm, double radiusMm)
            => _sm.CreateCircleByRadius(
                Mm.ToMeters(cxMm), Mm.ToMeters(cyMm), 0,
                Mm.ToMeters(radiusMm));

        /// <summary>
        /// Дуга, заданная центром, начальной и конечной точками и направлением
        /// (1 — против часовой стрелки, −1 — по часовой).
        /// </summary>
        public ISketchSegment Arc(
            double cxMm, double cyMm,
            double startXMm, double startYMm,
            double endXMm, double endYMm,
            short direction = 1)
            => _sm.CreateArc(
                Mm.ToMeters(cxMm), Mm.ToMeters(cyMm), 0,
                Mm.ToMeters(startXMm), Mm.ToMeters(startYMm), 0,
                Mm.ToMeters(endXMm), Mm.ToMeters(endYMm), 0,
                direction);

        public ISketchPoint Point(double xMm, double yMm)
            => _sm.CreatePoint(Mm.ToMeters(xMm), Mm.ToMeters(yMm), 0);

        public void Esc() => _doc.ClearSelection2(true);

        /// <summary>
        /// Smart-размер: предварительно нужно выделить сегменты, после чего
        /// вызывается AddDimension в точке размещения подписи.
        /// </summary>
        public IDisplayDimension Dimension(double xMm, double yMm, double valueMm)
        {
            var dim = _doc.AddDimension2(Mm.ToMeters(xMm), Mm.ToMeters(yMm), 0)
                      as IDisplayDimension;
            if (dim != null)
                ((IDimension)dim.GetDimension2(0)).SetSystemValue3(
                    Mm.ToMeters(valueMm),
                    (int)swInConfigurationOpts_e.swThisConfiguration,
                    null);
            return dim;
        }

        public IDisplayDimension RadialDimension(double xMm, double yMm, double radiusMm)
        {
            var dim = _doc.AddRadialDimension2(Mm.ToMeters(xMm), Mm.ToMeters(yMm), 0)
                      as IDisplayDimension;
            if (dim != null)
                ((IDimension)dim.GetDimension2(0)).SetSystemValue3(
                    Mm.ToMeters(radiusMm),
                    (int)swInConfigurationOpts_e.swThisConfiguration,
                    null);
            return dim;
        }

        public IDisplayDimension DiameterDimension(double xMm, double yMm, double diameterMm)
        {
            var dim = _doc.AddDiameterDimension2(Mm.ToMeters(xMm), Mm.ToMeters(yMm), 0)
                      as IDisplayDimension;
            if (dim != null)
                ((IDimension)dim.GetDimension2(0)).SetSystemValue3(
                    Mm.ToMeters(diameterMm),
                    (int)swInConfigurationOpts_e.swThisConfiguration,
                    null);
            return dim;
        }

        /// <summary>
        /// Помечает выбранный сегмент линии вспомогательным (осью симметрии).
        /// </summary>
        public void MakeConstruction(ISketchSegment segment)
        {
            segment.ConstructionGeometry = true;
        }
    }
}
