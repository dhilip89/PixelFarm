/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

//Apache2, 2017-2018, WinterDev
namespace PixelFarm.Drawing.DrawingBuffer
{
    public enum ResamplingAlgorithm
    {
        NearestNeighbor,
        Bilinear,
        Bicubic,
        SuperSampling
    }
}
