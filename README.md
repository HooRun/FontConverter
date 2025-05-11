![Font Converter For LVGL](https://github.com/HooRun/FontConverter/blob/master/Images/repository-open-graph.png?raw=true)

# Font Converter
 

### ![Font Converter Logo](https://github.com/HooRun/FontConverter/blob/master/FontConverter.Package/Images/Square44x44Logo.altform-lightunplated_targetsize-16.png?raw=true) Why was Font Converter created?

For converting fonts with large sizes and high resolutions (eight bits per pixel), LVGL's built-in converter works very well, and there's no need for another converter. The need for a new font converter arose when I wanted to convert fonts for a scrolling LED display. Since my display was monochrome, with each pixel corresponding to a single LED, I needed to set the resolution to one bit per pixel. After converting the font, I realized it wasn't suitable for my needs at all, especially with its small size. I decided to create an application where, during font conversion, I could preview the characters and the rendered bitmaps for each character and even edit them. That's why I started writing this font converter for LVGL.
