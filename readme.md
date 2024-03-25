## BinarySearchTree & BinaryNode
The BinarySearchTree and BinaryNode files contain the logic for binary search tree datastructure.

## Camera
The script connected to the Camera prefab. Handles to game logic regarding the Camera, including calculating its view in the floorplan.

## DesiredObject & Entrance
These are two objected connected to the desired object and entrance prefabs, containing the connected vertex.

## Vertex, PolygonVertex & Edge
These three scripts are the basic datastructures used by most algorithms. Vertex is simply a class with the x and y coordinates as properties. The PolygonVertex class inherits the Vertex class but also adds to which PolygonVertex an edge should be drawn. Edge is a class that contains the nodes it connects.

## Floorplan
Floorplan is a class connected to the floorplan prefab. It stores the information of the floorplan like PolygonVertices making up the floorplan, desired object, etc. It also contains a lot of the logic related to a floorplan like calculating what the cameras can see, how much the cameras can see and if a path exists from the entrance to the desired object.

## GameManager
The GameManager script handles general game logic, like you would for most games in Unity.

## LevelConfigManager & LevelConfig
The LevelConfigManager stores the chosen LevelConfig, since Unity does not allow the passing of arguments between scenes. The LevelConfig class is the configuration of a level as parsed from the json config file by the LevelConfigManager. This class contains the vertices, edges, desired object vertex and the entrance vertex to be used when creating a floorplan.
