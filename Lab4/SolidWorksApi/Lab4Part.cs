using System;
using SolidWorks.Interop.sldworks;

namespace SolidWorksApi
{
    /// <summary>
    /// Параметры детали лаб. №4. Все значения в миллиметрах.
    /// </summary>
    public sealed class Lab4Parameters
    {
        /// <summary>Внешний радиус «крюка» слева. По умолчанию 20.</summary>
        public double OuterArcRadius { get; set; } = 20;

        /// <summary>Внутренний радиус «крюка». По умолчанию 10.</summary>
        public double InnerArcRadius { get; set; } = 10;

        /// <summary>Высота прорези посередине «крюка». По умолчанию 20.</summary>
        public double SlotHeight { get; set; } = 20;

        /// <summary>Высота нижней опорной части. По умолчанию 40.</summary>
        public double BottomHeight { get; set; } = 40;

        /// <summary>Высота верхней площадки от центра отверстия. По умолчанию 25.</summary>
        public double TopHeight { get; set; } = 25;

        /// <summary>Полная высота детали. По умолчанию 80.</summary>
        public double TotalHeight { get; set; } = 80;

        /// <summary>Ширина левой части верхней площадки. По умолчанию 40.</summary>
        public double TopLeftWidth { get; set; } = 40;

        /// <summary>Ширина правой части верхней площадки. По умолчанию 30.</summary>
        public double TopRightWidth { get; set; } = 30;

        /// <summary>Ширина нижней опоры от оси «крюка» вправо. По умолчанию 15.</summary>
        public double BottomStepRight { get; set; } = 15;

        /// <summary>Расстояние от оси «крюка» влево по нижней грани. По умолчанию 20.</summary>
        public double BottomStepLeft { get; set; } = 20;

        /// <summary>Общая ширина нижней опоры. По умолчанию 45.</summary>
        public double BottomTotalWidth { get; set; } = 45;

        /// <summary>Диаметр сквозного отверстия в верхней площадке. По умолчанию 20.</summary>
        public double HoleDiameter { get; set; } = 20;

        /// <summary>Имя плоскости, на которой будет создан эскиз.</summary>
        public string SketchPlane { get; set; } = "Спереди";
    }

    /// <summary>
    /// Построитель эскиза детали из лаб. №4 (контур кронштейна с «крюком»
    /// слева и площадкой с отверстием справа).
    /// </summary>
    public static class Lab4PartBuilder
    {
        public static void Build(SolidWorksHost host, Lab4Parameters p)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));
            if (p == null) throw new ArgumentNullException(nameof(p));
            if (host.ModelDoc == null) host.NewPart();

            host.StartSketchOn(p.SketchPlane);
            var sb = new SketchBuilder(host);

            // Геометрический разбор. Точка (0, 0) — низ оси «крюка» по центру.
            // Влево от оси крюк уходит на p.OuterArcRadius (R20), а вправо
            // примыкает прямоугольная площадка.
            double hookCenterX = 0;
            double hookCenterY = p.BottomHeight;             // центр дуг = 40 мм над низом
            double hookOuterX = -p.OuterArcRadius;           // левая крайняя точка R20

            // ---- Контур внешней границы (по часовой стрелке начиная с низа) ----
            // 1. нижняя грань: от низа крюка до правого края (стрелка вправо).
            double rightX = p.BottomStepRight + p.TopRightWidth;
            sb.Line(hookCenterX, 0, rightX, 0);

            // 2. правый вертикальный край опоры до уровня верхней площадки.
            double topRectBottomY = p.BottomHeight + p.SlotHeight; // верх «крюка»
            sb.Line(rightX, 0, rightX, p.TotalHeight);

            // 3. верхняя грань площадки.
            double topLeftX = rightX - (p.TopLeftWidth + p.TopRightWidth);
            sb.Line(rightX, p.TotalHeight, topLeftX, p.TotalHeight);

            // 4. левая стенка верхней площадки до выхода на «крюк».
            sb.Line(topLeftX, p.TotalHeight, topLeftX, topRectBottomY);

            // 5. горизонтальная грань выходит к внешнему контуру «крюка».
            sb.Line(topLeftX, topRectBottomY, hookCenterX, topRectBottomY);

            // 6. внешняя дуга R20 (полуокружность слева).
            sb.Arc(
                cxMm: hookCenterX, cyMm: hookCenterY,
                startXMm: hookCenterX, startYMm: topRectBottomY,
                endXMm: hookCenterX, endYMm: 0,
                direction: 1);

            // ---- Внутренняя прорезь (слот) ----
            // Слот «открыт» вправо: левая часть — полуокружность R10, две
            // горизонтальных линии справа выходят на правую стенку площадки.
            double slotTopY = hookCenterY + p.InnerArcRadius;
            double slotBottomY = hookCenterY - p.InnerArcRadius;
            double slotStartX = hookCenterX; // центр совпадает с осью крюка

            // нижняя горизонталь слота
            sb.Line(slotStartX, slotBottomY, p.BottomStepRight, slotBottomY);
            // правая вертикаль (внутренний край площадки)
            sb.Line(p.BottomStepRight, slotBottomY, p.BottomStepRight, slotTopY);
            // верхняя горизонталь слота
            sb.Line(p.BottomStepRight, slotTopY, slotStartX, slotTopY);
            // полуокружность R10 слева — замыкает слот
            sb.Arc(
                cxMm: hookCenterX, cyMm: hookCenterY,
                startXMm: slotStartX, startYMm: slotTopY,
                endXMm: slotStartX, endYMm: slotBottomY,
                direction: -1);

            // ---- Отверстие Ø20 в правой части верхней площадки ----
            double holeCx = rightX - p.TopRightWidth / 2.0;
            double holeCy = p.TotalHeight - p.TopHeight;
            sb.Circle(holeCx, holeCy, p.HoleDiameter / 2.0);

            // ---- Размеры (Smart-dimension, AddRadial/Diameter) ----
            try
            {
                sb.RadialDimension(hookOuterX - 6, hookCenterY + 6, p.OuterArcRadius);
                sb.RadialDimension(slotStartX - 4, hookCenterY + 4, p.InnerArcRadius);
                sb.DiameterDimension(holeCx + 8, holeCy + 8, p.HoleDiameter);
            }
            catch
            {
                // В некоторых конфигурациях SolidWorks AddRadial требует
                // явного выделения дуги — при ошибке просто пропускаем.
            }

            host.ExitSketch();
            host.Rebuild();
        }
    }
}
