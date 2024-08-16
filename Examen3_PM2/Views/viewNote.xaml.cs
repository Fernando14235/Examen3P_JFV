using Examen3_PM2.Views;
using Examen3_PM2.ViewModels;
using System;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace Examen3_PM2.Views;

public partial class viewNote : ContentPage
{
	public viewNote()
	{
		InitializeComponent();

        BindingContext = new ViewModels.ViewNotesModel(Navigation);
    }
}