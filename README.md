<table border="0">
<tr>
<td>
<img src="https://github.com/cureos/accord/raw/portable/Setup/Portable/NuGet/portable-accord.png" alt="Portable Accord.NET logo" height="108" />
</td>
<td />
<td>
<img src="https://github.com/cureos/aforge/raw/master/Setup/Portable/NuGet/portable-aforge.png" alt="Portable AForge.NET logo" height="108" />
</td>
</tr>
</table>

# Portable Accord and AForge Samples

Copyright (c) 2009-2015 César Roberto de Souza (Accord.NET samples) and (c) 2010-2012 Andrew Kirillow (glyph recognition sample); adaptations to non-.NET platforms (c) 2013-2015 Anders Gustafsson, Cureos AB.  
Distributed under the Lesser GNU Public License, LGPL, version 3.0 (Accord.NET samples) or GNU Public License, GPL, version 3.0 (glyph recognition sample).

## Sample applications

### Image stitching

Windows Phone 8.1 adaptation of César's [image stitching sample application on CodeProject](http://www.codeproject.com/Articles/95453/Automatic-Image-Stitching-with-Accord-NET).

![Image stitching sample](/Files/panorama.png)

### Face detection

*Xamarin Forms* adaptation of César's [face detection sample application on CodeProject](http://www.codeproject.com/Articles/441226/Haar-feature-Object-Detection-in-Csharp).

Apps for these platforms:

* Android
* iOS
* Windows 8.1
* Windows Phone 8.1 (non-Silverlight)

The Windows 8.1 and Windows Phone 8.1 apps are relying on the pre-alpha release of Xamarin Forms for Windows and are currently not fully functional.

![Face detection sample](http://3.bp.blogspot.com/-fNN4Vl_muJo/VEgYN32B1II/AAAAAAAAAI4/fXIp5fmEVbo/s1600/facedetection.png)

### Corners detection

Windows 8.1 adaptation of the *Corners detection (SURF)* sample from Accord.NET.

![Corners detection sample](/Files/corners.png)

### Clustering

Windows Phone 8.1 adaptation of the *Clustering (Gaussian Mixture Models)* sample from Accord.NET.

![Clustering sample](/Files/clustering.png)

### Kinematics

.NET WPF adaptation of the *Denavit-Hartenberg Kinematics* sample from Accord.NET.

![Kinematics sample](/Files/kinematics.png)

### Wavelets

Android adaptation of the *Wavelets Transform*  from Accord.NET.

![Wavelets sample](/Files/wavelets.png)

### Glyph recognition

Windows 8.1 and Windows Phone 8.1 adaptations of the glyph recognition prototype in Andrew Kirillow's [GRATF](http://www.aforgenet.com/projects/gratf/) project, including a PCL/Windows 8.1 adaptation of the Image Processing Prototyper application from the *AForge.NET Framework*.

![Glyph recognition sample](/Files/glyph.png)

![Glyph recognition sample for Phone](/Files/glyph-phone.png)

## Notes on commercial use

The *Shim.Drawing* assemblies that are required to build the Portable Class Library versions of *AForge.NET Framework* and *Accord.NET Framework* are published under the General Public License, version 3.

There are *Shim* and *Shim.Drawing* assemblies available for *Xamarin.Android* and *Xamarin.iOS*, making it possible to 
incorporate *Portable AForge* and *Portable Accord* assemblies in *Android* and *iPhone* or *iPad* apps.

*Shim Drawing* is available for evaluation from *NuGet* for all supported platforms. For non-evaluation copies of *Shim Drawing*, please contact [Cureos](mailto:licenses@cureos.com).

Please also note that *AForge.NET Framework* (on which *Accord.NET Framework* is dependent) is licensed under LGPL version 3, and the copyright holder states the following on the *AForge.NET Framework* website:

> Regarding collaboration, contribution, offers, partnering, custom work/consulting, none GPL/LGPL licensing, etc., please, contact using the next e-mail:
aforge.net [at] gmail {dot} com

The *Accord.NET Framework* is released under LGPL version 2.1, and further licensing details can be found [here](http://accord-framework.net/license.html).
