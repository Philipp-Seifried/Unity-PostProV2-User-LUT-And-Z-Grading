# Z-Based Color Grading and Separate User LUT for Unity's "Post-Processing Stack V2"

Unity's Post-Processing Stack V2 comes with three modes for color grading: LDR, HDR and External LUTs. Unfortunately, HDR mode lacks the option to add a user-defined LUT, and the External LUT mode requires you to color grade in HDR, which Photoshop is not very good at. If you want to grade in Photoshop and use image filters such as curves (or do something funky like posterizing the image), you're left with the LDR color grading mode in Unity, which doesn't apply tonemapping and simply clips color values higher than 1.0

This package works around this limitation by implementing a separate color grading filter, similar to the V1 post-processing stack's "User LUT" effect. Simply add a regular Post-Processing V2 Color Grading effect, set it to HDR and use it to do tonemapping, then add the "User LUT" effect from this package and supply it with an LDR LUT that will be applied after the tone-mapping stage.

The "User LUT" effect in this package also allows you to specify a separate LUT for grading the background (based on the camera's depth texture), so you can apply different color grading for distant objects.

The downside of doing all this in a separate effect is that it comes at a slightly higher cost, since the effect isn't part of the Post-Processing Stack's uber shader, which handles many of Unity's post-processing effects in one pass.

## Getting Started

Drop the code from the "UserLUT" folder into your project.
*IMPORTANT*: For the effect to work in a build, make sure that the supplied UserLUT shader is included in the build.

Add a Post Process Volume and Layer to your camera, if you haven't already. Add a Color Grading effect to the Post Processing Stack, select "High Definition Range" for mode and select the tonemapping of your choice. Leave any other checkboxes/parameters of the Color Grading effect unchecked/default.

Add the effect from this package to the stack ("Custom/UserLUT").

## Parameters

* *LUT* - a 1024x32 RGB24 texture (256x16 LUTs should also be compatible). Some sample LUTs for testing are provided in the LUTs folder. To author your own, take a screenshot of your game after tonemapping, without any additional color grading applied. Open the image in Photoshop (or your image processing software of choice) and add the NeutralLUT_32 Texture in the image's corner. Apply any image filters that operate on single pixels (e.g. color balance, but not blur), then crop and export the resulting LUT into your project (settings: no sRGB, no mipmaps, no compression). Once you set your LUT in the "User LUT" effect, your game should look like the processed screenshot in Photoshop.
* *Blend* - lets you blend between your selected LUT and neutral color grading (1 = full effect of your LUT).
* *Use Background LUT* - if this is checked, the image is graded based on each pixel's depth in the camera's depth texture, and all background-related parameters take effect.
* *Background LUT* - color grading lookup table for distant parts of the image.
* *Background Blend Start* - at this distance (in world units), the color grading starts blending between foreground and background
* *Background Blend Range* - distance after "Background Blend Start" at which the Background LUT has full effect.

## Compatibility

Has been tested with Unity 2018.3 and 2019.1 on PC only, but should work on any platform that supports the Post-Processing Stack V2.
Compatible with the Standard rendering pipeline, LWRP and HDRP.
Compatible with Gamma and linear lighting.
Orthographic cameras have trouble with the *Background Blend Start* and *Background Blend Range* parameters. Z-Blending works in principle, but the units are off and for now you need to experiment to set the right blending distance parameters.
Not tested in VR.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Authors

* [Philipp Seifried](https://twitter.com/PhilippSeifried) - Much of the code was adapted from the Post-Processing Stack V1 and V2 versions of Color Grading / User LUT.

