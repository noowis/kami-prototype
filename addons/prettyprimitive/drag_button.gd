@tool
extends Button

@export var drag_text : String


var plugin
var primitives

func _ready() -> void:
	primitives = get_parent()
	while primitives != null:
		if primitives.name == "Primitives":
			break
		primitives = primitives.get_parent()
	plugin = primitives.plugin # ... there's surely a better way to find this!!
	
	tooltip_text = "Click, OR drag from this button to the viewport to drop into scene"


func _get_drag_data(at_position: Vector2) -> Variant:
	print("Getting drag data from ", drag_text)
	var results: Dictionary = plugin.GetDragObject(drag_text)
	print(results, " ", drag_text)
	var obj = results["csg"] if results["csg"] else results["mesh"]
	add_child(obj)
	
	var drag_payload = {
		"from": self,
		"type": "nodes",
		"nodes": [ obj.get_path() ]
	}
	
	var preview = Label.new()
	preview.text = drag_text
	set_drag_preview(preview)
	
	plugin.StartDragging(results, drag_text)
	print("p: ", plugin.is_dragging)
	return drag_payload
