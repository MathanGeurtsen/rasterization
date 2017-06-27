Architechture:
For the scene graph we store all the to be loaded meshes in a List<Mesh>
All the meshes now have their own model matrix, transform matrix, texture, rotation speed for turning around their own axis, rotation speed for turning around their parent and a name. We gave them all a rotation speed as our scene
is a view of earth and the moon orbiting the earth. We chose to store
the parent for each mesh instead of all children for a mesh, so every
mesh would only have 1 variable for a relation instead of a List<>,
and it made rotation relative to their parent a bit easier to implement. The scene graph is stored in the
SceneGraph class, it contains a List<> storing the multiple lights we
have, a projection matrix, 2 view matrices (one for movement and
turning around the Y-axis, and 1 for turning around the X-axis).

We've chosen multiple matrices so the camera movement would feel
natural and familiar to gamers used to First Person Shooters. This way
we can introduce	pitch and yaw without introducing roll. The camera
will now move in the worlds XZ plane with WASD and the world Y axis
with q and e instead of  movement being bound to the position and
rotation of the camera. The scenegraph has a float value a which is
used to rotate the objects.

The render method in the scene graph sets the correct model matrix for
every mesh through the toWorldSpace method, The toWorldSpace method is
a recursive method to transform the model matrix of every object which
were placed relative to their parent to world space coordinates. 

We calculate a transform for every mesh by multiplying their model
matrices with the view matrices and the projection matrix in the scene graph and store the result in a mesh's transform.
Rotation and moving the camera are done by applying translations/rotations on the view matrices.
To reset the view of the camera we change both view matrices to the
identy matrix4. For controls we use a method which is called every
tick to check if a button is pressed, see the control plan below.
The mesh tree for printing in the console is build up recursively by
checking which meshes correspond to which parent and the tree is then
printed in reverse order to correctly show the relationship between
parent and child mesh.

We have implemented the PHONG shading model with parameters we have
set out in the fragment shader. We have implemented two light sources and
each lightsource has four variables which determine its makeup, namely
lightPos, lightColor, specularStr and diffStr. The lightPos is ultimately
controlled by the modelMatrix and rotation speed that it has. The
modelMatrix is given to it's constructor wich we call in
initMeshes(). the rotation speed is standard 1 but can be
modified. Every light as a location in world space that follows from
the modelmatrix that is given in initMeshes(), which is transferred
into the shader uniform lightposN, where N stands for the number of
the lightsource we want to control.

The lightColor, specularStr and diffStr respectively control the
intensity of the lights at the three colors, the strength of the
specular shading and the strength of the diffuse shading. Of course we
have the ambient shading which isn't bound to a particular
lightsource, which has its own variable ambientLight.

in the fragment post processing shader we add a vignetting effect as well as chromatic aberration,
controlled by the distance to the center of the screen as evident by the darkened edges on the screen and the red and blue ofsets around objects.

Control map:
-Movement:
  >"W" to go forward
  >"A" to go left
  >"S" to go backwards
  >"D" to go right
  >"Q" to go up
  >"E" to go down
-Look:
  >"Up" to look up
  >"Left" to look left
  >"Down" to look down
  >"Right" to look right
-Modifiers:
  >"+" to speed up Movement and Look actions
  >"-" to slow down Movement and Look actions
  >"R" to reset the camera to the default position
f-Output:
  >"P" to print the Obj tree
