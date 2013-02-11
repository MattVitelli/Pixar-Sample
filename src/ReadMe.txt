Roto-Photo
An application for generating rotoscoped / cartoony images using a combination of machine learning and image processing algorithms.
Copyright (C) 2013 Matt Vitelli

-----------
How To Use:
-----------

1)	Build/Run the application.
2)	Go to File->Open (Ctrl + O)
3)	Select a File
4)	Play around with the parameters on the left.
	The parameters are as follows:
	Outline Sharpness - Sets how sharp/apparent the cartoon outlines are in the image
	Color Smoothness -	Sets how much smoothing is performed before initializing the K-Means algorithm.
						This roughly corresponds to how smooth the image appears
	Number of Colors -	Sets how many colors are used in the final image. More colors generally produces
						more realistic images, whereas fewer colors produces more stylized images.
	Show Original Image-Enables/disables drawing the original source image
5)	Go to File->Save (Ctrl + S) to save the file as a PNG

---------------
How To Install:
---------------
To install / run this code, you need the following:
1) Microsoft Visual Studio C# 2008
2) Microsoft XNA Game Studio 3.1

There's also a binary in the bin folder.
You must install xnafx31_redist.exe before
running Roto-Photo