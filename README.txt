2023-08-22 Mark Kaldas

UNITY VERSION: 2022.3.4f1

CONTROLS:
 - WASD to move
 - SHIFT to sprint
 - SPACE to jump
 - TAB to toggle inventory
 - MOUSE to look around
 - LEFT CLICK to place the current block
 - RIGHT CLICK to remove the block you're looking at
 - R to rotate the current block
 - MOUSE SCROLLWHEEL to move the selected block closer or farther from you
 
Here's my prototype. I was busy the second day, so I had to complete this in only one day. The art is programmer
art of course, and the UI is not as slick as I would like, but the block placing mechanic is extremely robust, and
the character controller is nice and responsive.

Included are the following features:
 - Block placing and removing, with rotation
 - An inventory system and block selection
 - A physics-based character controller with jumping and sprinting
 - Special block shapes as described in the document
 - Support for non-cubic blocks and blocks with arbitrary models, shapes, and shaders

How it works is that each block has a set of hand-placed connection surfaces that are used to determine if a block
can be placed in a given location, and a set of blocking zones defining the volume of the block, which needs to be
clear of other blocks in order to place it. This way of doing it is extremely flexible and allows practically any
kind of block to be placed, no matter how weird its shape.

Raycasting and boxcasting makes little sense from both a gameplay and technical perspective given the weird shapes 
described in the document, so I opted for a more robust (if not slightly annoying) solution, which is that the player 
will simply place blocks N metres in front of them, where N is adjustable with the scroll wheel. This way you can 
place blocks easily no matter what awkward angle you're looking at them from, and you have total control over the exact 
position it'll be placed at.

The blocks are defined in JSON in the resources folder, and the block models and preview models are loaded from
there too, as well as the icons. The green and red translucent block preview is done with a custom surface shader. 
(Evidently, I'm using the standard render pipeline).

One thing to note is that performance-wise, this is definitely not scalable at the moment. If you wanted to expand
this prototype to a full game, I'm afraid a dedicated voxel engine is the way to go if you want to allow the player
to place hundreds or thousands of blocks while keeping performance and memory usage reasonable. Obviously, that's out 
of the question for just a prototype made in 48 (24, actually) hours, so I used this method instead.

Thanks for reading, and have fun with the prototype!