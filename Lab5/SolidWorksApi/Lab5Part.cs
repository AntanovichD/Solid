using System;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidWorksApi
{
    /// <summary>
    /// Параметры детали лаб. №5 (все размеры в миллиметрах).
    /// </summary>
    public sealed class Lab5Parameters
    {
        // ---- Шаг 1: основание ----
        public double BaseLength { get; set; } = 160;   // 160 — длина основания
        public double BaseWidth { get; set; } = 85;     //  85 — глубина основания
        public double BaseHeight { get; set; } = 55;    //  55 — высота основания

        // ---- Шаг 2: «гребень» сверху с трапециевидной крышей ----
        public double FinBottomLength { get; set; } = 80; // 80 — нижняя длина гребня
        public double FinTopLength { get; set; } = 60;    // 60 — верхняя длина гребня
        public double FinHeight { get; set; } = 45;       // 45 — высота гребня
        public double FinWidth { get; set; } = 85;        // глубина равна основанию

        // ---- Шаг 3: вертикальный паз сбоку ----
        public double SlotWidth { get; set; } = 25;   // 25 — ширина паза
        public double SlotHeight { get; set; } = 35;  // 35 — высота паза
        public double SlotOffsetX { get; set; } = 0;  // позиция паза вдоль X
        public double SlotDepth { get; set; } = 80;   // глубина выреза вдоль Y

        // ---- Шаг 4: полукруглый вырез ----
        public double NotchRadius { get; set; } = 30; // R30
        public double NotchDepth { get; set; } = 85;  // насквозь по глубине
    }

    /// <summary>
    /// Пошаговое построение детали из лаб. №5. Каждая «ступень» — отдельная
    /// фича: основание → гребень → паз → полукруглый вырез. Метод
    /// <see cref="BuildAll"/> выполняет все шаги подряд.
    /// </summary>
    public sealed class Lab5PartBuilder
    {
        private readonly SolidWorksHost _host;
        private readonly FeatureBuilder _fb;

        public IFeature BaseFeature { get; private set; }
        public IFeature FinFeature { get; private set; }
        public IFeature SlotFeature { get; private set; }
        public IFeature NotchFeature { get; private set; }

        public Lab5PartBuilder(SolidWorksHost host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _fb = new FeatureBuilder(host);
        }

        public void BuildAll(Lab5Parameters p)
        {
            BuildStep1(p);
            BuildStep2(p);
            BuildStep3(p);
            BuildStep4(p);
        }

        /// <summary>
        /// Шаг 1. Основание: прямоугольник <see cref="Lab5Parameters.BaseLength"/>
        /// × <see cref="Lab5Parameters.BaseHeight"/> на плоскости «Спереди»,
        /// симметричное выдавливание (Средняя плоскость) на
        /// <see cref="Lab5Parameters.BaseWidth"/>.
        /// </summary>
        public IFeature BuildStep1(Lab5Parameters p)
        {
            _host.StartSketchOn("Спереди");
            var sb = new SketchBuilder(_host);

            double halfX = p.BaseLength / 2.0;
            // прямоугольник: левая нижняя в (-halfX, 0), правая верхняя в (halfX, BaseHeight)
            sb.Line(-halfX, 0, halfX, 0);
            sb.Line(halfX, 0, halfX, p.BaseHeight);
            sb.Line(halfX, p.BaseHeight, -halfX, p.BaseHeight);
            sb.Line(-halfX, p.BaseHeight, -halfX, 0);

            _host.ExitSketch();

            BaseFeature = _fb.Extrude(ExtrudeOptions.MidPlane(p.BaseWidth));
            _host.ClearSelection();
            _host.Rebuild();
            return BaseFeature;
        }

        /// <summary>
        /// Шаг 2. Гребень на верхней грани основания: трапеция (нижнее
        /// основание FinBottomLength, верхнее FinTopLength, высота FinHeight),
        /// выдавливание симметрично на FinWidth.
        /// </summary>
        public IFeature BuildStep2(Lab5Parameters p)
        {
            if (BaseFeature == null)
                throw new InvalidOperationException(
                    "Сначала выполните шаг 1 — основание не построено.");

            // Эскиз на верхней грани (face index 5 — индекс зависит от
            // построения; для прямоугольного бруска через MidPlane обычно
            // 4 — верх, но используем плоскость напрямую для надёжности).
            _host.StartSketchOn("Сверху");
            var sb = new SketchBuilder(_host);

            double halfBottom = p.FinBottomLength / 2.0;
            double halfTop = p.FinTopLength / 2.0;
            double y = p.FinHeight;

            // трапеция: нижние углы у Y=0, верхние у Y=FinHeight, сужается
            // симметрично вдоль X.
            sb.Line(-halfBottom, 0, halfBottom, 0);
            sb.Line(halfBottom, 0, halfTop, y);
            sb.Line(halfTop, y, -halfTop, y);
            sb.Line(-halfTop, y, -halfBottom, 0);

            _host.ExitSketch();

            FinFeature = _fb.Extrude(ExtrudeOptions.MidPlane(p.FinWidth));
            _host.ClearSelection();
            _host.Rebuild();
            return FinFeature;
        }

        /// <summary>
        /// Шаг 3. Прямоугольный паз 25 × 35 на левой боковой грани детали,
        /// насквозь.
        /// </summary>
        public IFeature BuildStep3(Lab5Parameters p)
        {
            if (BaseFeature == null)
                throw new InvalidOperationException(
                    "Сначала выполните шаги 1 и 2.");

            _host.StartSketchOn("Слева");
            var sb = new SketchBuilder(_host);

            // паз начинается у верхней грани основания и идёт вниз
            double half = p.SlotWidth / 2.0;
            double topY = p.BaseHeight + p.FinHeight;
            double bottomY = topY - p.SlotHeight;
            double cx = p.SlotOffsetX;

            sb.Line(cx - half, topY, cx + half, topY);
            sb.Line(cx + half, topY, cx + half, bottomY);
            sb.Line(cx + half, bottomY, cx - half, bottomY);
            sb.Line(cx - half, bottomY, cx - half, topY);

            _host.ExitSketch();

            SlotFeature = _fb.Cut(new ExtrudeOptions
            {
                EndCondition1 = swEndConditions_e.swEndCondThroughAll
            });
            _host.ClearSelection();
            _host.Rebuild();
            return SlotFeature;
        }

        /// <summary>
        /// Шаг 4. Полукруглый вырез R30 на передней грани основания.
        /// Эскиз: полуокружность плюс хорда — насквозь.
        /// </summary>
        public IFeature BuildStep4(Lab5Parameters p)
        {
            if (BaseFeature == null)
                throw new InvalidOperationException(
                    "Сначала выполните шаги 1–3.");

            _host.StartSketchOn("Спереди");
            var sb = new SketchBuilder(_host);

            // полуокружность опирается на нижнюю кромку (Y=0).
            double cx = 0;
            double cy = 0;
            double r = p.NotchRadius;

            // дуга идёт против часовой стрелки от (r, 0) к (-r, 0)
            sb.Arc(
                cxMm: cx, cyMm: cy,
                startXMm: cx + r, startYMm: cy,
                endXMm: cx - r, endYMm: cy,
                direction: 1);
            // хорда (нижняя горизонталь)
            sb.Line(cx - r, cy, cx + r, cy);

            _host.ExitSketch();

            NotchFeature = _fb.Cut(new ExtrudeOptions
            {
                EndCondition1 = swEndConditions_e.swEndCondThroughAll
            });
            _host.ClearSelection();
            _host.Rebuild();
            return NotchFeature;
        }
    }
}
