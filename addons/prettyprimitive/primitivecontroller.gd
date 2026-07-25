@tool
extends Control

var plugin

func _ready():
	if Engine.is_editor_hint():
		_on_XYZ_toggled(%BoxesXYZ.button_pressed)
		_on_XY_toggled(%PlaneXY.button_pressed)
	
	var mat_picker = %Material/MaterialPicker
	mat_picker.custom_minimum_size = Vector2(100,0)
	mat_picker.base_type = "Material"
	mat_picker.edited_resource = load("res://addons/prettyprimitive/Materials/default.tres")


func SetPlugin(_plugin: EditorPlugin) -> void:
	plugin = _plugin

enum Parent {
	None,
	Rigid,
	Static,
	AreaParent,
	SpatialParent
	}

# Return { axis:int, nodes_type:Nodes }
func GetOptions():
	return { "axis":          %AxisOptions.selected,
			"parent_type":    %Options/ParentOptions.selected,
			"with_mesh" :     %Options/Mesh.button_pressed,
			"with_collider" : %Options/Collider.button_pressed,
			"as_csg" :        %Options/AsCSG.button_pressed,
			"with_mat" :      %WithMat.button_pressed,
			"with_color" :    %WithColor.button_pressed
			}


func _on_XYZ_toggled(button_pressed):
	if button_pressed:
		%BoxesSize.prefix = "X:"
		%BoxesY.visible = true
		%BoxesZ.visible = true
	else:
		%BoxesSize.prefix = "Size:"
		%BoxesY.visible = false
		%BoxesZ.visible = false

func _on_XY_toggled(button_pressed):
	if button_pressed:
		%PlaneSize.prefix = "X:"
		%PlaneY.visible = true
	else:
		%PlaneSize.prefix = "Size:"
		%PlaneY.visible = false

func _on_with_color_toggled(button_pressed):
	if button_pressed:
		%WithMat.button_pressed = false

func _on_with_mat_toggled(button_pressed):
	if button_pressed:
		%WithColor.button_pressed = false

# Select for color if you change the color
func _on_color_picker_button_color_changed(color):
	%WithColor.button_pressed = true
	%WithMat.button_pressed = false

# Don't allow plane or capsule when csg
func _on_as_csg_toggled(button_pressed):
	%MakePlane.disabled = button_pressed
	%MakeCapsule.disabled = button_pressed

func _on_MakeCube_pressed():
	var results = MakeCube()
	AddToParents(results, "Cube")

func _on_MakeSphere_pressed():
	var results = MakeSphere()
	AddToParents(results, "Sphere")

func _on_MakeCapsule_pressed():
	var results = MakeCapsule()
	AddToParents(results, "Capsule")

func _on_MakeCylinder_pressed():
	var results = MakeCylinder()
	AddToParents(results, "Cylinder")

func _on_MakeCone_pressed():
	var results = MakeCone()
	AddToParents(results, "Cone")

func _on_MakePlane_pressed():
	var results = MakePlane()
	AddToParents(results, "Plane")


## Return the parents in the scene tree that will be parents to the new object.
## Returns { owner, parents:[dest1, dest2, ... ] }
func GetDestParents():
	var scene_root = get_tree().edited_scene_root
	var new_owner = scene_root.get_tree().edited_scene_root
	
	var dest
	var sel = EditorInterface.get_selection().get_selected_nodes()
	if sel.size() > 0:
		dest = sel
	else:
		dest = [ scene_root ]
		
	return { "owner": new_owner, "parents": dest }


func AdjustMaterial(obj, options):
	if !options: options = GetOptions()
	print("adding material to ", obj)
	if options["with_color"]: AddColor(obj)
	elif options["with_mat"]: AddMaterial(obj)

func AddColor(obj):
	var mat = StandardMaterial3D.new()
	mat.albedo_color = %ColorPickerButton.color
	if obj is MeshInstance3D:
		print("AddColor to ", obj, ", mesh: ", obj.mesh)
		obj.set_surface_override_material(0, mat)
	else:
		obj.material = mat #assume csg

func AddMaterial(obj):
	var mat = %Material/MaterialPicker.edited_resource
	if mat == null:
		print_debug("Trying to use material, but no material set!")
		return
	if not mat is Material:
		print_debug("Resource must be material!")
	if obj is MeshInstance3D:
		obj.set_surface_override_material(0, mat)
	else:
		obj.material = mat #assume csg


## Install node shells into the tree.
# Return [ top, meshinstance (without mesh), collider (without shape), null (placeholder) ]
# Top is the new top, possible an existing node when "No parent", or else
# will be a new RigidDynamicBody3d, StaticBody3D, Area3D, or Node3D. In that
# case, top will become a child of parent.
func BuildBase(options, parent, theowner, csg, pos_ref: Node3D, undoredo: EditorUndoRedoManager):
	#print ("buildbase col: ", options["with_collider"] )
	
	var themesh : MeshInstance3D = null
	var col = null
	
	if not options["as_csg"]:
		if options["with_mesh"]: themesh = MeshInstance3D.new()
		if options["with_collider"]: col = CollisionShape3D.new()
	
	var pnt = parent
	match options["parent_type"]:
		Parent.Rigid:
			pnt = RigidBody3D.new()
		Parent.Static:
			pnt = StaticBody3D.new()
		Parent.AreaParent:
			pnt = Area3D.new()
		Parent.SpatialParent:
			pnt = Node3D.new()
			
	var is_new_parent = (pnt != parent)
	
	if is_new_parent:
		print("--3333333")
		undoredo.add_do_method(parent, "add_child", pnt)
		undoredo.add_do_property(pnt, "owner", theowner)
		undoredo.add_do_reference(pnt)
		undoredo.add_undo_method(parent, "remove_child", pnt)
		#undoredo.add_undo_reference()
		print("--444444")
		
		#parent.add_child(pnt)
		#pnt.owner = theowner
		if pos_ref:
			#pnt.global_position = pos_ref.global_position
			undoredo.add_do_property(pnt, "global_position", pos_ref.global_position)
	if themesh:
		print("11111 ", theowner)
		undoredo.add_do_method(pnt, "add_child", themesh)
		undoredo.add_do_property(themesh, "owner", theowner)
		undoredo.add_do_reference(themesh)
		undoredo.add_undo_method(pnt, "remove_child", themesh)
		print("2222")
		
		#pnt.add_child(themesh)
		if pos_ref and not is_new_parent:
			#themesh.global_position = pos_ref.global_position
			undoredo.add_do_property(themesh, "global_position", pos_ref.global_position)
	if col:
		#pnt.add_child(col)
		#col.owner = theowner
		undoredo.add_do_method(pnt, "add_child", col)
		undoredo.add_do_reference(col)
		undoredo.add_do_property(col, "owner", theowner)
		
		if pos_ref and not is_new_parent:
			#col.global_position = pos_ref.global_position
			undoredo.add_do_property(col, "global_position", pos_ref.global_position)
	if csg:
		#pnt.add_child(csg)
		#csg.owner = theowner
		undoredo.add_do_method(pnt, "add_child", csg)
		undoredo.add_do_reference(csg)
		undoredo.add_do_property(csg, "owner", theowner)
		if pos_ref and not is_new_parent:
			#csg.global_position = pos_ref.global_position
			undoredo.add_do_property(csg, "global_position", pos_ref.global_position)
	
	print("build base done")
	return { "parent": pnt,
			 "is_new_parent": is_new_parent,
			 "meshi": themesh,
			 "collider": col,
			 "csg": csg
			}

## Configure and push the new nodes onto the scene.
# objects is like: { "parent": null, "mesh": cube, "shape": shape, "csg": csg }
# dest is: { "owner", "parents": [ ...nodes already in scene... ] }
func AddToParents(objects, type, dest = null, pos_ref : Node3D = null):
	if dest == null: dest = GetDestParents()
	var options = GetOptions()
	
	var csg     = objects.csg
	var meshi   = objects.mesh
	var shape   = objects.shape
	
	print("creating undo action")
	var undoredo : EditorUndoRedoManager = plugin.get_undo_redo()
	undoredo.create_action("Added primitives")
	
	for p in dest["parents"]:
		var dup = null
		if options.as_csg:
			dup = csg if (p == dest["parents"][0]) else csg.duplicate()
		
		var nodes = BuildBase(options, p, dest["owner"], dup, pos_ref, undoredo)
		if nodes.is_new_parent:
			#nodes.parent.name = type
			undoredo.add_do_property(nodes.parent, "name", type)
		
		if nodes.meshi:
			if meshi is MeshInstance3D: # was probably a drag and drop
				nodes.meshi.mesh = meshi.mesh
				#nodes.meshi.global_position = meshi.global_position
				undoredo.add_do_property(nodes.meshi, "global_position", meshi.global_position)
				#nodes[1].mesh = meshi.mesh
			else:
				nodes.meshi.mesh = meshi
			#nodes.meshi.name = type + "Mesh"
			undoredo.add_do_property(nodes.meshi, "name", type + "Mesh")
			AdjustMaterial(nodes.meshi, options)
		
		if nodes.collider: #collider
			nodes.collider.shape = shape
			nodes.collider.name = type + "Shape"
		
		if nodes.csg:
			nodes.csg.name = type + "CSG"
			AdjustMaterial(nodes.csg, options)
		
		AlignObject(type, nodes, options["axis"])
	
	print("before commit")
	undoredo.commit_action()
	print("after commit")

func AlignObject(type, nodes, alignment):
		if alignment == -1:
			var options = GetOptions()
			alignment = options["axis"]
		if type == "Capsule":
			match alignment:
				0:
					if nodes.meshi: nodes.meshi.rotation = Vector3(0,0,PI/2)
					if nodes.collider: nodes.collider.rotation = Vector3(0,0,PI/2)
				1:
					if nodes.meshi: nodes.meshi.rotation = Vector3(0,0,0)
					if nodes.collider: nodes.collider.rotation = Vector3(0,0,0)
				2:
					if nodes.meshi: nodes.meshi.rotation = Vector3(PI/2,0,0)
					if nodes.collider: nodes.collider.rotation = Vector3(PI/2,0,0)
		elif type == "Cylinder" or type == "Cone":
			match alignment:
				2:
					if nodes.meshi: nodes.meshi.rotation = Vector3(0,PI/2,PI/2)
					if nodes.collider: nodes.collider.rotation = Vector3(0,PI/2,PI/2)
					if nodes.csg: nodes.csg.rotation = Vector3(0,PI/2,PI/2)
				0:
					if nodes.meshi: nodes.meshi.rotation = Vector3(0,0,PI/2)
					if nodes.collider: nodes.collider.rotation = Vector3(0,0,PI/2)
					if nodes.csg: nodes.csg.rotation = Vector3(0,0,PI/2)
		elif type == "Plane":
			if alignment == 2:
				if nodes.meshi: nodes.meshi.rotation = Vector3(0,PI/2,PI/2)
				if nodes.collider: nodes.collider.rotation = Vector3(0,PI/2,PI/2)
			elif alignment == 0:
				if nodes.meshi: nodes.meshi.rotation = Vector3(0,0,-PI/2)
				if nodes.collider: nodes.collider.rotation = Vector3(0,0,-PI/2)


func MakeCube():
	var sizex = %BoxesSize.value
	var sizey = %BoxesY.value
	var sizez = %BoxesZ.value
	if not %BoxesXYZ.button_pressed:
		sizey = sizex
		sizez = sizex
	
	var options = GetOptions()
	var csg
	var cube = null
	var shape = null

	if options["as_csg"]:
		csg = CSGBox3D.new()
		csg.size = Vector3(sizex, sizey, sizez)
		if options["with_collider"]:
			csg.use_collision = true
	else:
		if options["with_mesh"]:
			cube = BoxMesh.new()
			cube.size = Vector3(sizex, sizey, sizez)
		if options["with_collider"]:
			shape = BoxShape3D.new()
			shape.extents = Vector3(sizex/2, sizey/2, sizez/2)
	
	return { "parent": null, "mesh": cube, "shape": shape, "csg": csg }


func MakeSphere():
	var size = %SphereSize.value
	
	var dest = GetDestParents()
	var options = GetOptions()
	
	var m = null
	var shape = null
	var csg = null
	if options["as_csg"]:
		csg = CSGSphere3D.new()
		csg.radius = size
		csg.rings = 8
		csg.radial_segments = 16
		if options["with_collider"]:
			csg.use_collision = true
	else:
		if options["with_mesh"]:
			m = SphereMesh.new()
			m.radius = size
			m.height = 2 * size
		if options["with_collider"]:
			shape = SphereShape3D.new()
			shape.radius = size
	
	return { "parent": null, "mesh": m, "shape": shape, "csg": csg }


func MakeCapsule():
	var height = %Capsule/Height.value
	var radius = %Capsule/Radius.value
	
	var options = GetOptions()
	
	var m = null
	if options["with_mesh"]:
		m = CapsuleMesh.new()
		m.radius = radius
		m.height = height
	var shape = null
	if options["with_collider"]:
		shape = CapsuleShape3D.new()
		shape.radius = radius
		shape.height = height # - 2*radius
	
	return { "parent": null, "mesh": m, "shape": shape, "csg": null }


func MakeCone():
	var height = %Cone/Height.value
	var r1 = %Cone/RadiusBottom.value
	var r2 = %Cone/RadiusTop.value
	return MakeBaseCylinder(height, r1, r2)

func MakeCylinder():
	var height = %Cylinder/Height.value
	var radius = %Cylinder/Radius.value
	return MakeBaseCylinder(height, radius, radius)

func MakeBaseCylinder(height, r1, r2):
	var options = GetOptions()
	var m
	var shape
	var csg
	var is_cone = (r1 == 0 || r2 == 0)
	if options["as_csg"]:
		csg = CSGCylinder3D.new()
		csg.cone = is_cone
		csg.height = height
		csg.radius = max(r1,r2)
		csg.sides = 16
		if options["with_collider"]:
			csg.use_collision = true
	else:
		if options["with_mesh"]:
			m = CylinderMesh.new()
			m.height = height
			m.bottom_radius = r1
			m.top_radius = r2
		if options["with_collider"]:
			if r1 == r2:
				shape = CylinderShape3D.new()
				shape.radius = r1
				shape.height = height
			else:
				shape = m.create_convex_shape()
	
	return { "parent": null, "mesh": m, "shape": shape, "csg": csg }


func MakePlane():
	var options = GetOptions()
	var sizex = %PlaneSize.value
	var sizey = %PlaneY.value
	if not %PlaneXY.button_pressed:
		sizey = sizex
		
	var m
	if options["with_mesh"]: 
		m = PlaneMesh.new()
		m.size = Vector2(sizex,sizey)
	var shape
	if options["with_collider"]:
		shape = BoxShape3D.new()
		shape.extents = Vector3(sizex/2, .005, sizey/2)
	
	return { "parent": null, "mesh": m, "shape": shape, "csg": null }
