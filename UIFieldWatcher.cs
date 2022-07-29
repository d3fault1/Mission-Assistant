using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MissionAssistant
{
    static class UIFieldWatcher
    {
        public static void BindMapLine(MapLine obj, int hitpoint)
        {
            UI.GUI.lineColors.SetBinding(ComboBox.SelectedValueProperty, new Binding { Source = obj, Path = new PropertyPath(MapLine.LineColorProperty), Mode = BindingMode.TwoWay, Converter = new UIColorComboboxConveter() });
            UI.GUI.lineType.SetBinding(ComboBox.SelectedIndexProperty, new Binding { Source = obj, Path = new PropertyPath(MapLine.LineDashArrayProperty), Mode = BindingMode.TwoWay, Converter = new UILineTypeConverter() });
            UI.GUI.lineThickness.SetBinding(TextBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(MapLine.LineThicknessProperty), Mode = BindingMode.TwoWay, Converter = new UIDoubleAsIsConverter() });
        }

        public static void BindCircle(Circle obj)
        {
            UI.GUI.circleRadius.SetBinding(TextBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(Circle.RadiusProperty), Mode = BindingMode.TwoWay, Converter = new UIDoubleAsIsConverter() });
            UI.GUI.circleThickness.SetBinding(TextBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(Circle.ThicknessProperty), Mode = BindingMode.TwoWay, Converter = new UIDoubleAsIsConverter() });
            UI.GUI.circleColors.SetBinding(ComboBox.SelectedValueProperty, new Binding { Source = obj, Path = new PropertyPath(Circle.ColorProperty), Mode = BindingMode.TwoWay, Converter = new UIColorComboboxConveter() });
            UI.GUI.circleType.SetBinding(ComboBox.SelectedIndexProperty, new Binding { Source = obj, Path = new PropertyPath(Circle.StrokeDashArrayProperty), Mode = BindingMode.TwoWay, Converter = new UILineTypeConverter() });
        }

        public static void BindPolygon(PolygonLine obj, int hitpoint)
        {
            var main = obj.Parent;
            UI.GUI.polygonColors.SetBinding(ComboBox.SelectedValueProperty, new Binding { Source = main, Path = new PropertyPath(Polygon.ColorProperty), Mode = BindingMode.TwoWay, Converter = new UIColorComboboxConveter() });
            UI.GUI.polygonType.SetBinding(ComboBox.SelectedIndexProperty, new Binding { Source = main, Path = new PropertyPath(Polygon.DashArrayProperty), Mode = BindingMode.TwoWay, Converter = new UILineTypeConverter() });
            UI.GUI.polygonThickness.SetBinding(TextBox.TextProperty, new Binding { Source = main, Path = new PropertyPath(Polygon.ThicknessProperty), Mode = BindingMode.TwoWay, Converter = new UIDoubleAsIsConverter() });
        }

        //Needs Work
        public static void BindImage(Image obj)
        {
            UI.GUI.imageHeight.SetBinding(TextBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(Image.HeightProperty), Mode = BindingMode.TwoWay });
            UI.GUI.imageWidth.SetBinding(TextBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(Image.WidthProperty), Mode = BindingMode.TwoWay });
        }

        public static void BindRoute(RouteLeg obj, int hitpoint)
        {
            var main = obj.Parent;
            RefreshRoutePanel(main.Aircraft);
            UI.GUI.routeAcNameBox.SetBinding(TextBox.TextProperty, new Binding { Source = main, Path = new PropertyPath(Route.AircraftProperty), Mode = BindingMode.TwoWay });
            UI.GUI.routeThickness.SetBinding(TextBox.TextProperty, new Binding { Source = main, Path = new PropertyPath(Route.ThicknessProperty), Mode = BindingMode.TwoWay, Converter = new UIDoubleAsIsConverter() });
            UI.GUI.routeType.SetBinding(ComboBox.SelectedIndexProperty, new Binding { Source = main, Path = new PropertyPath(Route.DashArrayProperty), Mode = BindingMode.TwoWay, Converter = new UILineTypeConverter() });
            UI.GUI.routeColors.SetBinding(ComboBox.SelectedValueProperty, new Binding { Source = main, Path = new PropertyPath(Route.ColorProperty), Mode = BindingMode.TwoWay, Converter = new UIColorComboboxConveter() });
            UI.GUI.msnNameBox.SetBinding(TextBox.TextProperty, new Binding { Source = main, Path = new PropertyPath(Route.MissionProperty), Mode = BindingMode.TwoWay });
            MultiBinding total_distance_binding = new MultiBinding { Mode = BindingMode.OneWay, Converter = new UIRouteDistanceFormat() };
            total_distance_binding.Bindings.Add(new Binding { Source = main, Path = new PropertyPath(Route.TotalDistanceProperty), Mode = BindingMode.OneWay });
            total_distance_binding.Bindings.Add(new Binding { Source = UI.GUI.routeTotalDistanceUnit, Path = new PropertyPath(ComboBox.SelectedValueProperty), Mode = BindingMode.OneWay, });
            UI.GUI.routeTotalDistanceBox.SetBinding(TextBox.TextProperty, total_distance_binding);
            UI.GUI.routeTotalTimeBox.SetBinding(TextBox.TextProperty, new Binding { Source = main, Path = new PropertyPath(Route.TotalTimeProperty), Mode = BindingMode.OneWay, Converter = new UITimeDataFormat() });
            UI.GUI.routeTotalFuelBox.SetBinding(TextBox.TextProperty, new Binding { Source = main, Path = new PropertyPath(Route.TotalFuelProperty), Mode = BindingMode.OneWay, Converter = new UIDoubleDataFormat() });
            UI.GUI.routeStartingFuelBox.SetBinding(ComboBox.TextProperty, new Binding { Source = main, Path = new PropertyPath(Route.StartingFuelProperty), Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.LostFocus, Converter = new UIDoubleAsIsConverter() });
            if (UI.GUI.routeStartingFuelBox.Text == "0")
            {
                UI.GUI.routeStartingFuelBox.SelectedIndex = 0;
                UI.GUI.routeStartingFuelBox.GetBindingExpression(ComboBox.TextProperty).UpdateSource();
            }
            UI.GUI.routeMinimaFuelBox.SetBinding(TextBox.TextProperty, new Binding { Source = main, Path = new PropertyPath(Route.MinimaProperty), Mode = BindingMode.TwoWay, Converter = new UIDoubleAsIsConverter() });
            if (hitpoint == 2) UI.GUI.routeCoordBox.SetBinding(TextBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(RouteLeg.EndPointProperty), Mode = BindingMode.OneWay, Converter = new UICoordFormat() });
            else UI.GUI.routeCoordBox.SetBinding(TextBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(RouteLeg.StartPointProperty), Mode = BindingMode.OneWay, Converter = new UICoordFormat() });
            UI.GUI.Ncoorddeg.SetBinding(TextBox.TextProperty, new Binding { Source = UI.GUI.routeCoordBox, Path = new PropertyPath(TextBox.TextProperty), Mode = BindingMode.OneWay, Converter = new UIInputCoordFormat(), ConverterParameter = 1 });
            UI.GUI.Ncoordmin.SetBinding(TextBox.TextProperty, new Binding { Source = UI.GUI.routeCoordBox, Path = new PropertyPath(TextBox.TextProperty), Mode = BindingMode.OneWay, Converter = new UIInputCoordFormat(), ConverterParameter = 2 });
            UI.GUI.Ncoordsec.SetBinding(TextBox.TextProperty, new Binding { Source = UI.GUI.routeCoordBox, Path = new PropertyPath(TextBox.TextProperty), Mode = BindingMode.OneWay, Converter = new UIInputCoordFormat(), ConverterParameter = 3 });
            UI.GUI.Ecoorddeg.SetBinding(TextBox.TextProperty, new Binding { Source = UI.GUI.routeCoordBox, Path = new PropertyPath(TextBox.TextProperty), Mode = BindingMode.OneWay, Converter = new UIInputCoordFormat(), ConverterParameter = 4 });
            UI.GUI.Ecoordmin.SetBinding(TextBox.TextProperty, new Binding { Source = UI.GUI.routeCoordBox, Path = new PropertyPath(TextBox.TextProperty), Mode = BindingMode.OneWay, Converter = new UIInputCoordFormat(), ConverterParameter = 5 });
            UI.GUI.Ecoordsec.SetBinding(TextBox.TextProperty, new Binding { Source = UI.GUI.routeCoordBox, Path = new PropertyPath(TextBox.TextProperty), Mode = BindingMode.OneWay, Converter = new UIInputCoordFormat(), ConverterParameter = 6 });
            UI.GUI.routeNameBox.SetBinding(TextBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(RouteLeg.NameProperty), Mode = BindingMode.TwoWay });
            UI.GUI.routeTypeBox.SetBinding(ComboBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(RouteLeg.TypeProperty), Mode = BindingMode.TwoWay, Converter = new UILegTypeConverter() });
            UI.GUI.routeTrackBox.SetBinding(TextBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(RouteLeg.TrackProperty), Mode = BindingMode.OneWay, Converter = new UIDoubleDataFormat() });
            MultiBinding distance_binding = new MultiBinding { Mode = BindingMode.OneWay, Converter = new UIRouteDistanceFormat() };
            distance_binding.Bindings.Add(new Binding { Source = obj, Path = new PropertyPath(RouteLeg.DistanceProperty), Mode = BindingMode.OneWay });
            distance_binding.Bindings.Add(new Binding { Source = UI.GUI.routeDistanceUnit, Path = new PropertyPath(ComboBox.SelectedValueProperty), Mode = BindingMode.OneWay });
            UI.GUI.routeDistanceBox.SetBinding(TextBox.TextProperty, distance_binding);
            UI.GUI.cddist.SetBinding(TextBox.TextProperty, new Binding { Source = obj.Segments[0], Path = new PropertyPath(RouteLegSegment.DistanceProperty), Mode = BindingMode.OneWay, Converter = new UIDoubleDataFormat() });
            UI.GUI.lvldist.SetBinding(TextBox.TextProperty, new Binding { Source = obj.Segments[1], Path = new PropertyPath(RouteLegSegment.DistanceProperty), Mode = BindingMode.OneWay, Converter = new UIDoubleDataFormat() });
            UI.GUI.lnddist.SetBinding(TextBox.TextProperty, new Binding { Source = obj.Segments[2], Path = new PropertyPath(RouteLegSegment.DistanceProperty), Mode = BindingMode.OneWay, Converter = new UIDoubleDataFormat() });
            UI.GUI.routeAltBox.SetBinding(ComboBox.SelectedItemProperty, new Binding { Source = obj, Path = new PropertyPath(RouteLeg.AltitudeProperty), Mode = BindingMode.TwoWay, Converter = new UIDoubleAsIsConverter() });
            if (UI.GUI.routeAltBox.SelectedIndex == -1)
            {
                UI.GUI.routeAltBox.SelectedIndex = 0;
                UI.GUI.routeAltBox.GetBindingExpression(ComboBox.SelectedItemProperty).UpdateSource();
            }
            UI.GUI.routeSpeedBox.SetBinding(ComboBox.SelectedItemProperty, new Binding { Source = obj, Path = new PropertyPath(RouteLeg.SpeedProperty), Mode = BindingMode.TwoWay, Converter = new UIDoubleAsIsConverter() });
            if (UI.GUI.routeSpeedBox.SelectedIndex == -1)
            {
                UI.GUI.routeSpeedBox.SelectedIndex = 0;
                UI.GUI.routeSpeedBox.GetBindingExpression(ComboBox.SelectedItemProperty).UpdateSource();
            }
            UI.GUI.routeTimeBox.SetBinding(TextBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(RouteLeg.TimeProperty), Mode = BindingMode.OneWay, Converter = new UITimeDataFormat() });
            UI.GUI.cdtime.SetBinding(TextBox.TextProperty, new Binding { Source = obj.Segments[0], Path = new PropertyPath(RouteLegSegment.TimeProperty), Mode = BindingMode.OneWay, Converter = new UITimeDataFormat() });
            UI.GUI.lvltime.SetBinding(TextBox.TextProperty, new Binding { Source = obj.Segments[1], Path = new PropertyPath(RouteLegSegment.TimeProperty), Mode = BindingMode.OneWay, Converter = new UITimeDataFormat() });
            UI.GUI.lndtime.SetBinding(TextBox.TextProperty, new Binding { Source = obj.Segments[2], Path = new PropertyPath(RouteLegSegment.TimeProperty), Mode = BindingMode.OneWay, Converter = new UITimeDataFormat() });
            UI.GUI.routeFuelBox.SetBinding(TextBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(RouteLeg.FuelProperty), Mode = BindingMode.OneWay, Converter = new UIDoubleDataFormat() });
            UI.GUI.cdfuel.SetBinding(TextBox.TextProperty, new Binding { Source = obj.Segments[0], Path = new PropertyPath(RouteLegSegment.FuelProperty), Mode = BindingMode.OneWay, Converter = new UIDoubleDataFormat() });
            UI.GUI.lvlfuel.SetBinding(TextBox.TextProperty, new Binding { Source = obj.Segments[1], Path = new PropertyPath(RouteLegSegment.FuelProperty), Mode = BindingMode.OneWay, Converter = new UIDoubleDataFormat() });
            UI.GUI.lndfuel.SetBinding(TextBox.TextProperty, new Binding { Source = obj.Segments[2], Path = new PropertyPath(RouteLegSegment.FuelProperty), Mode = BindingMode.OneWay, Converter = new UIDoubleDataFormat() });
            UI.GUI.routeRefuelDefuelBox.SetBinding(TextBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(RouteLeg.FuelAdjustmentProperty), Mode = BindingMode.TwoWay, Converter = new UIFuelAdjustConverter(), ConverterParameter = UI.GUI.routeRefuelDefuelSelector });
            UI.GUI.routecheckpoints.ItemsSource = obj.Checkpoints;
        }

        public static void BindRoute(DiversionLeg obj, int hitpoint)
        {
            var main = obj.Parent;
            RefreshRoutePanel(main.Aircraft);
            UI.GUI.routeAcNameBox.SetBinding(TextBox.TextProperty, new Binding { Source = main, Path = new PropertyPath(Route.AircraftProperty), Mode = BindingMode.TwoWay });
            UI.GUI.routeThickness.SetBinding(TextBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(DiversionLeg.LineThicknessProperty), Mode = BindingMode.TwoWay, Converter = new UIDoubleAsIsConverter() });
            UI.GUI.routeType.SetBinding(ComboBox.SelectedIndexProperty, new Binding { Source = obj, Path = new PropertyPath(DiversionLeg.LineDashArrayProperty), Mode = BindingMode.TwoWay, Converter = new UILineTypeConverter() });
            UI.GUI.routeColors.SetBinding(ComboBox.SelectedValueProperty, new Binding { Source = obj, Path = new PropertyPath(DiversionLeg.LineColorProperty), Mode = BindingMode.TwoWay, Converter = new UIColorComboboxConveter() });
            UI.GUI.msnNameBox.SetBinding(TextBox.TextProperty, new Binding { Source = main, Path = new PropertyPath(Route.MissionProperty), Mode = BindingMode.TwoWay });
            MultiBinding total_distance_binding = new MultiBinding { Mode = BindingMode.OneWay, Converter = new UIRouteDistanceFormat() };
            total_distance_binding.Bindings.Add(new Binding { Source = main, Path = new PropertyPath(Route.TotalDistanceProperty), Mode = BindingMode.OneWay });
            total_distance_binding.Bindings.Add(new Binding { Source = UI.GUI.routeTotalDistanceUnit, Path = new PropertyPath(ComboBox.SelectedValueProperty), Mode = BindingMode.OneWay, });
            UI.GUI.routeTotalDistanceBox.SetBinding(TextBox.TextProperty, total_distance_binding);
            UI.GUI.routeTotalTimeBox.SetBinding(TextBox.TextProperty, new Binding { Source = main, Path = new PropertyPath(Route.TotalTimeProperty), Mode = BindingMode.OneWay, Converter = new UITimeDataFormat() });
            UI.GUI.routeTotalFuelBox.SetBinding(TextBox.TextProperty, new Binding { Source = main, Path = new PropertyPath(Route.TotalFuelProperty), Mode = BindingMode.OneWay, Converter = new UIDoubleDataFormat() });
            UI.GUI.routeStartingFuelBox.SetBinding(ComboBox.TextProperty, new Binding { Source = main, Path = new PropertyPath(Route.StartingFuelProperty), Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.LostFocus, Converter = new UIDoubleAsIsConverter() });
            if (UI.GUI.routeStartingFuelBox.Text == "0")
            {
                UI.GUI.routeStartingFuelBox.SelectedIndex = 0;
                UI.GUI.routeStartingFuelBox.GetBindingExpression(ComboBox.TextProperty).UpdateSource();
            }
            UI.GUI.routeMinimaFuelBox.SetBinding(TextBox.TextProperty, new Binding { Source = main, Path = new PropertyPath(Route.MinimaProperty), Mode = BindingMode.TwoWay, Converter = new UIDoubleAsIsConverter() });
            if (hitpoint == 2) UI.GUI.routeCoordBox.SetBinding(TextBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(DiversionLeg.EndPointProperty), Mode = BindingMode.OneWay, Converter = new UICoordFormat() });
            else UI.GUI.routeCoordBox.SetBinding(TextBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(DiversionLeg.StartPointProperty), Mode = BindingMode.OneWay, Converter = new UICoordFormat() });
            UI.GUI.Ncoorddeg.SetBinding(TextBox.TextProperty, new Binding { Source = UI.GUI.routeCoordBox, Path = new PropertyPath(TextBox.TextProperty), Mode = BindingMode.OneWay, Converter = new UIInputCoordFormat(), ConverterParameter = 1 });
            UI.GUI.Ncoordmin.SetBinding(TextBox.TextProperty, new Binding { Source = UI.GUI.routeCoordBox, Path = new PropertyPath(TextBox.TextProperty), Mode = BindingMode.OneWay, Converter = new UIInputCoordFormat(), ConverterParameter = 2 });
            UI.GUI.Ncoordsec.SetBinding(TextBox.TextProperty, new Binding { Source = UI.GUI.routeCoordBox, Path = new PropertyPath(TextBox.TextProperty), Mode = BindingMode.OneWay, Converter = new UIInputCoordFormat(), ConverterParameter = 3 });
            UI.GUI.Ecoorddeg.SetBinding(TextBox.TextProperty, new Binding { Source = UI.GUI.routeCoordBox, Path = new PropertyPath(TextBox.TextProperty), Mode = BindingMode.OneWay, Converter = new UIInputCoordFormat(), ConverterParameter = 4 });
            UI.GUI.Ecoordmin.SetBinding(TextBox.TextProperty, new Binding { Source = UI.GUI.routeCoordBox, Path = new PropertyPath(TextBox.TextProperty), Mode = BindingMode.OneWay, Converter = new UIInputCoordFormat(), ConverterParameter = 5 });
            UI.GUI.Ecoordsec.SetBinding(TextBox.TextProperty, new Binding { Source = UI.GUI.routeCoordBox, Path = new PropertyPath(TextBox.TextProperty), Mode = BindingMode.OneWay, Converter = new UIInputCoordFormat(), ConverterParameter = 6 });
            UI.GUI.routeNameBox.SetBinding(TextBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(DiversionLeg.NameProperty), Mode = BindingMode.TwoWay });
            UI.GUI.routeTypeBox.SetBinding(ComboBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(DiversionLeg.TypeProperty), Mode = BindingMode.OneWay, Converter = new UILegTypeConverter() });
            UI.GUI.routeTrackBox.SetBinding(TextBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(DiversionLeg.TrackProperty), Mode = BindingMode.OneWay, Converter = new UIDoubleDataFormat() });
            MultiBinding distance_binding = new MultiBinding { Mode = BindingMode.OneWay, Converter = new UIRouteDistanceFormat() };
            distance_binding.Bindings.Add(new Binding { Source = obj, Path = new PropertyPath(DiversionLeg.DistanceProperty), Mode = BindingMode.OneWay });
            distance_binding.Bindings.Add(new Binding { Source = UI.GUI.routeDistanceUnit, Path = new PropertyPath(ComboBox.SelectedValueProperty), Mode = BindingMode.OneWay });
            UI.GUI.routeDistanceBox.SetBinding(TextBox.TextProperty, distance_binding);
            UI.GUI.cddist.Text = @"N\A";
            UI.GUI.cddist.IsEnabled = false;
            UI.GUI.lvldist.SetBinding(TextBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(DiversionLeg.DistanceProperty), Mode = BindingMode.OneWay, Converter = new UIDoubleDataFormat() });
            UI.GUI.lnddist.Text = @"N\A";
            UI.GUI.lnddist.IsEnabled = false;
            UI.GUI.routeAltBox.SetBinding(ComboBox.SelectedItemProperty, new Binding { Source = obj, Path = new PropertyPath(DiversionLeg.AltitudeProperty), Mode = BindingMode.TwoWay, Converter = new UIDoubleAsIsConverter() });
            if (UI.GUI.routeAltBox.SelectedIndex == -1)
            {
                UI.GUI.routeAltBox.SelectedIndex = 0;
                UI.GUI.routeAltBox.GetBindingExpression(ComboBox.SelectedItemProperty).UpdateSource();
            }
            UI.GUI.routeSpeedBox.SetBinding(ComboBox.SelectedItemProperty, new Binding { Source = obj, Path = new PropertyPath(DiversionLeg.SpeedProperty), Mode = BindingMode.TwoWay, Converter = new UIDoubleAsIsConverter() });
            if (UI.GUI.routeSpeedBox.SelectedIndex == -1)
            {
                UI.GUI.routeSpeedBox.SelectedIndex = 0;
                UI.GUI.routeSpeedBox.GetBindingExpression(ComboBox.SelectedItemProperty).UpdateSource();
            }
            UI.GUI.routeTimeBox.SetBinding(TextBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(DiversionLeg.TimeProperty), Mode = BindingMode.OneWay, Converter = new UITimeDataFormat() });
            UI.GUI.cdtime.Text = @"N\A";
            UI.GUI.cdtime.IsEnabled = false;
            UI.GUI.lvltime.SetBinding(TextBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(DiversionLeg.TimeProperty), Mode = BindingMode.OneWay, Converter = new UITimeDataFormat() });
            UI.GUI.lndtime.Text = @"N\A";
            UI.GUI.lndtime.IsEnabled = false;
            UI.GUI.routeFuelBox.SetBinding(TextBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(DiversionLeg.FuelProperty), Mode = BindingMode.OneWay, Converter = new UIDoubleDataFormat() });
            UI.GUI.cdfuel.Text = @"N\A";
            UI.GUI.cdfuel.IsEnabled = false;
            UI.GUI.lvlfuel.SetBinding(TextBox.TextProperty, new Binding { Source = obj, Path = new PropertyPath(DiversionLeg.FuelProperty), Mode = BindingMode.OneWay, Converter = new UIDoubleDataFormat() });
            UI.GUI.lndfuel.Text = @"N\A";
            UI.GUI.lndfuel.IsEnabled = false;
            UI.GUI.routeRefuelDefuelBox.Text = @"N\A";
            UI.GUI.routeRefuelDefuelBox.IsEnabled = false;
            UI.GUI.routeRefuelDefuelSelector.IsEnabled = false;
            UI.GUI.routecheckpoints.ItemsSource = obj.Checkpoints;
        }

        public static void RefreshRoutePanel(string aircraft)
        {
            if (string.IsNullOrEmpty(aircraft)) return;
            UI.GUI.routeAcNameBox.Text = aircraft;
            var dist_unit = DataCalculations.GetAircraftUnits(aircraft, "distance");
            var alt_unit = DataCalculations.GetAircraftUnits(aircraft, "alt");
            var speed_unit = DataCalculations.GetAircraftUnits(aircraft, "speed");
            var fuel_unit = DataCalculations.GetAircraftUnits(aircraft, "fuel");
            UI.GUI.routeTotalDistanceUnit.SelectedValue = dist_unit;
            UI.GUI.routeTotalFuelUnit.Text = fuel_unit;
            UI.GUI.routeStartingFuelUnit.Text = fuel_unit;
            UI.GUI.routeMinimaFuelUnit.Text = fuel_unit;
            UI.GUI.routeDistanceUnit.SelectedValue = dist_unit;
            UI.GUI.routeAltUnit.Text = alt_unit;
            UI.GUI.routeSpeedUnit.Text = speed_unit;
            UI.GUI.routeFuelUnit.Text = fuel_unit;
            UI.GUI.routeRefuelDefuelUnit.Text = fuel_unit;

            UI.GUI.routeStartingFuelBox.ItemsSource = DataCalculations.GetAircraftFuelConfigurations(aircraft);
            UI.GUI.routeAltBox.ItemsSource = DataCalculations.GetAircraftAltitudes(aircraft);
            UI.GUI.routeSpeedBox.ItemsSource = DataCalculations.GetAircraftSpeeds(aircraft);
            UI.GUI.routeStartingFuelBox.Items.Refresh();
            UI.GUI.routeAltBox.Items.Refresh();
            UI.GUI.routeSpeedBox.Items.Refresh();
        }

        public static void Unbind(string mode)
        {
            switch (mode)
            {
                case "line":
                    BindingOperations.ClearBinding(UI.GUI.lineColors, ComboBox.SelectedValueProperty);
                    BindingOperations.ClearBinding(UI.GUI.lineType, ComboBox.SelectedIndexProperty);
                    BindingOperations.ClearBinding(UI.GUI.lineThickness, TextBox.TextProperty);
                    break;
                case "circle":
                    BindingOperations.ClearBinding(UI.GUI.circleRadius, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.circleThickness, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.circleColors, ComboBox.SelectedValueProperty);
                    BindingOperations.ClearBinding(UI.GUI.circleType, ComboBox.SelectedIndexProperty);
                    break;
                case "polygon":
                    BindingOperations.ClearBinding(UI.GUI.polygonColors, ComboBox.SelectedValueProperty);
                    BindingOperations.ClearBinding(UI.GUI.polygonType, ComboBox.SelectedIndexProperty);
                    BindingOperations.ClearBinding(UI.GUI.polygonThickness, TextBox.TextProperty);
                    break;
                case "image":
                    BindingOperations.ClearBinding(UI.GUI.imageHeight, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.imageWidth, TextBox.TextProperty);
                    break;
                case "route":
                    BindingOperations.ClearBinding(UI.GUI.routeAcNameBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeThickness, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeColors, ComboBox.SelectedValueProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeType, ComboBox.SelectedIndexProperty);
                    BindingOperations.ClearBinding(UI.GUI.msnNameBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeTotalDistanceBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeTotalTimeBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeTotalFuelBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeStartingFuelBox, ComboBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeStartingFuelBox, ComboBox.SelectedIndexProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeMinimaFuelBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeCoordBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.Ncoorddeg, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.Ncoordmin, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.Ncoordsec, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.Ecoorddeg, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.Ecoordmin, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.Ecoordsec, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeNameBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeTypeBox, ComboBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeTrackBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeDistanceBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.cddist, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.lvldist, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.lnddist, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeAltBox, ComboBox.SelectedItemProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeAltBox, ComboBox.SelectedIndexProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeSpeedBox, ComboBox.SelectedItemProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeSpeedBox, ComboBox.SelectedIndexProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeTimeBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.cdtime, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.lvltime, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.lndtime, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeFuelBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.cdfuel, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.lvlfuel, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.lndfuel, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeRefuelDefuelBox, TextBox.TextProperty);
                    break;
                case "diversion":
                    BindingOperations.ClearBinding(UI.GUI.routeAcNameBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeThickness, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeColors, ComboBox.SelectedValueProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeType, ComboBox.SelectedIndexProperty);
                    BindingOperations.ClearBinding(UI.GUI.msnNameBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeTotalDistanceBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeTotalTimeBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeTotalFuelBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeStartingFuelBox, ComboBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeStartingFuelBox, ComboBox.SelectedIndexProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeMinimaFuelBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeCoordBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.Ncoorddeg, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.Ncoordmin, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.Ncoordsec, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.Ecoorddeg, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.Ecoordmin, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.Ecoordsec, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeNameBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeTypeBox, ComboBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeTrackBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeDistanceBox, TextBox.TextProperty);
                    UI.GUI.cddist.IsEnabled = true;
                    BindingOperations.ClearBinding(UI.GUI.lvldist, TextBox.TextProperty);
                    UI.GUI.lnddist.IsEnabled = true;
                    BindingOperations.ClearBinding(UI.GUI.routeAltBox, ComboBox.SelectedItemProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeAltBox, ComboBox.SelectedIndexProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeSpeedBox, ComboBox.SelectedItemProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeSpeedBox, ComboBox.SelectedIndexProperty);
                    BindingOperations.ClearBinding(UI.GUI.routeTimeBox, TextBox.TextProperty);
                    UI.GUI.cdtime.IsEnabled = true;
                    BindingOperations.ClearBinding(UI.GUI.lvltime, TextBox.TextProperty);
                    UI.GUI.lndtime.IsEnabled = true;
                    BindingOperations.ClearBinding(UI.GUI.routeFuelBox, TextBox.TextProperty);
                    UI.GUI.cdfuel.IsEnabled = true;
                    BindingOperations.ClearBinding(UI.GUI.lvlfuel, TextBox.TextProperty);
                    UI.GUI.lndfuel.IsEnabled = true;
                    UI.GUI.routeRefuelDefuelBox.IsEnabled = true;
                    UI.GUI.routeRefuelDefuelSelector.IsEnabled = true;
                    break;
            }
            
        }
    }
}
