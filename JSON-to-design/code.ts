// This plugin will allow users to upload JSON data and process it within Figma.

// This file holds the main code for plugins. Code in this file has access to
// the *figma document* via the figma global object.
// You can access browser APIs in the <script> tag inside "ui.html" which has a
// full browser environment (See https://www.figma.com/plugin-docs/how-plugins-run).

// This shows the HTML page in "ui.html".
figma.showUI(__html__);
// Calls to "parent.postMessage" from within the HTML page will trigger this
// callback. The callback will be passed the "pluginMessage" property of the
// posted message.
figma.ui.onmessage = async (msg: {type: string, prompt: string}) => {
  // One way of distinguishing between different types of messages sent from
  // your HTML page is to use an object with a "type" property like this.
  if (msg.type === 'process-json') {
    try {
      // Here you can process the JSON data however you need
      const res = await fetch('http://localhost:8000/figma_plugin', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({"prompt": msg.prompt})
      })
      const data = await res.json()
      console.log(data)
      
      const frame = figma.createFrame() 
      //resize(width, height)
      frame.resize(250, 500)
      frame.x = 0
      frame.y = 0
      frame.fills = data.frame.fills as Paint[]
      frame.name = data.frame.name

      data.frame.children.forEach((child: any) => {
        const rect = figma.createRectangle()
        rect.name = child.name
        rect.resize(child.width, child.height)
        rect.x = child.x
        rect.y = child.y
        rect.fills = child.fills
        frame.appendChild(rect)
      })

      
      figma.currentPage.appendChild(frame)
      figma.currentPage.selection = [frame]
      figma.viewport.scrollAndZoomIntoView([frame])

      // Notify success
      figma.notify('JSON data processed successfully');
    } catch (error: any) {
      console.log(error.message)

      // Handle any errors
      figma.notify('Error processing JSON: ' + error);
    }
  }

  // Make sure to close the plugin when you're done. Otherwise the plugin will
  // keep running, which shows the cancel button at the bottom of the screen.
  figma.closePlugin();
};
