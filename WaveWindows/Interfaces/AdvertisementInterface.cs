using System;

namespace WaveWindows.Interfaces;

internal class AdvertisementInterface
{
	internal string Image { get; set; }

	internal string Link { get; set; }

	internal static AdvertisementInterface Random()
	{
		return new Random().Next(6) switch
		{
			0 => new AdvertisementInterface
			{
				Image = "Includes/Banners/Lootlabs.png",
				Link = ""
			}, 
			1 => new AdvertisementInterface
			{
				Image = "Includes/Banners/Linkvertise.jpg",
				Link = ""
			}, 
			2 => new AdvertisementInterface
			{
				Image = "Includes/Banners/OceanKeys.png",
				Link = ""
			}, 
			3 => new AdvertisementInterface
			{
				Image = "Includes/Banners/ProjectRain.png",
				Link = ""
			}, 
			4 => new AdvertisementInterface
			{
				Image = "Includes/Banners/WavePremium.png",
				Link = ""
			}, 
			5 => new AdvertisementInterface
			{
				Image = "Includes/Banners/WiiHub.png",
				Link = ""
			}, 
			_ => throw new ArgumentOutOfRangeException("AdvertisementInterface.Random"), 
		};
	}
}
