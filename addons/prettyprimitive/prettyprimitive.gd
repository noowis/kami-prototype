@tool
extends EditorPlugin


var dock

var is_dragging := false
var drag_type : String
var drag_objects
var drag_object
var drag_overlay: Control
var last_pos : Vector2
var last_distance := 5.0


func _enter_tree():
	# Initialization of the plugin goes here
	# Load the dock scene and instance it
	dock = preload("res://addons/prettyprimitive/panel.tscn").instantiate()
	dock.SetPlugin(self)

	# Add the loaded scene to the docks
	# Note that LEFT_UL means the left of the editor, upper-left dock
	add_control_to_dock(DOCK_SLOT_RIGHT_BL, dock)
	
	drag_overlay = Control.new()
	drag_overlay.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	drag_overlay.mouse_filter = Control.MOUSE_FILTER_IGNORE # Ignore until drag starts
	drag_overlay.gui_input.connect(_on_overlay_gui_input)
	
	# Inject the overlay directly over the 3D editor's screen viewport area
	var vp = EditorInterface.get_editor_viewport_3d()
	if vp:
		#print("Adding overlay")
		vp.add_child(drag_overlay)


func _exit_tree():
	# Remove the dock
	remove_control_from_docks(dock)
	dock.free()
	
	if drag_overlay:
		drag_overlay.queue_free()


func _notification(what: int) -> void:
	if not is_dragging: return
	
	if what == NOTIFICATION_DRAG_END:
		print("Notified drag end")
		StopDragging()

func _process(delta: float) -> void:
	if is_dragging:
		if not Input.is_mouse_button_pressed(MOUSE_BUTTON_LEFT):
			print("turning off dragging in process")
			StopDragging()

		else:
			var pos = get_viewport().get_mouse_position()
			if pos != last_pos:
				#print("dragging ", pos)
				last_pos = pos
				UpdateHover()

func GetDragObject(type: String):
	var results: Dictionary
	match type:
		"Cube":     results = dock.MakeCube()
		"Sphere":   results = dock.MakeSphere()
		"Capsule":  results = dock.MakeCapsule()
		"Cylinder": results = dock.MakeCylinder()
		"Cone":     results = dock.MakeCone()
		"Plane":    results = dock.MakePlane()
	
	if results["mesh"]:
		var mi = MeshInstance3D.new()
		mi.name = type
		mi.mesh = results["mesh"]
		print("mesh: ", mi.mesh, "  results: ", results)
		results["mesh"] = mi
		dock.AdjustMaterial(mi, null)
		dock.AlignObject(type, { "meshi": mi, "csg": null, "collider": null }, -1)
	elif results["csg"]:
		dock.AdjustMaterial(results["csg"], null)
		dock.AlignObject(type, { "meshi": null, "csg": results["csg"], "collider": null }, -1)
	return results

func UpdateHover():
	var camera := EditorInterface.get_editor_viewport_3d().get_camera_3d()
	
	var pos := camera.get_viewport().get_mouse_position()
	if not camera.get_viewport().get_visible_rect().has_point(pos):
		drag_object.visible = false
		return
	
	drag_object.visible = true
	
	var world := camera.get_world_3d() #EditorInterface.get_editor_viewport_3d().world_3d
	var space_state := world.direct_space_state
	var start := camera.global_position
	var max_cast := 25.0
	var end   := project_mouse(camera, max_cast)
	var params := PhysicsRayQueryParameters3D.new()
	params.from = start
	params.to = end
	params.collision_mask = ~0 #current_obj.raycast_collision_mask
	var ray_result : Dictionary = space_state.intersect_ray(params)

	var hit_pos : Vector3 #= last_raycast_pos
	var hit_normal : Vector3 #= last_raycast_normal
	
	if !ray_result.is_empty():
		hit_pos = ray_result.position
		hit_normal = ray_result.normal
		last_distance = (hit_pos - camera.global_position).length()
		drag_object.global_position = hit_pos #+ Vector3(0,5,0)
		#current_obj.last_raycast_pos = hit_pos
	else:
		var v := (params.to - params.from).normalized()
		hit_pos = camera.global_position + last_distance * v
		drag_object.global_position = hit_pos
		

# Project mouse position onto a plane that is distance away from the camera origin at its closest point.
# Note this is NOT the final distance between camera and projected point.
func project_mouse(camera: Camera3D, distance: float, offset: Vector2 = Vector2.ZERO) -> Vector3:
	return camera.project_position(camera.get_viewport().get_mouse_position() + offset, distance)



func StartDragging(objects, type):
	is_dragging = true
	drag_type = type
	drag_objects = objects
	drag_object = objects.csg if objects.csg else objects.mesh
	drag_overlay.mouse_filter = Control.MOUSE_FILTER_STOP

func StopDragging():
	is_dragging = false
	if drag_object != null:
		#print("path on delete: ", drag_object.get_path())
		if drag_object.visible:
			var dests = { "owner": get_tree().edited_scene_root,
						  "parents": [ get_tree().edited_scene_root ]
						}
			dock.AddToParents(drag_objects, drag_type, dests, drag_object)
			drag_object.queue_free()
		else:
			drag_object.queue_free()
		drag_object = null
		drag_objects = null
	print ("dragging stopped")


func _on_overlay_gui_input(viewport_camera: Camera3D, event: InputEvent) -> int:
	print('tick') # ***** this isn't called because the drag absorbs events??
	if !is_dragging: return false
	
	if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_LEFT and not event.pressed:
		drop_mesh(viewport_camera, event.position)
		StopDragging()
		return true
	
	if event is InputEventMouseMotion:
		if event.is_pressed():
			print("overlay gui")
		else:
			print("turning off dragging")
			StopDragging()
	
	return false

func drop_mesh(camera: Camera3D, position: Vector2) -> void:
	print("DROP!!")
