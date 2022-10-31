require 'ids.lua'

local function ModelRenderable(model, tint)
{
	local renderable = NewObject('UiRenderable')
	local modelElem = NewObject('DisplayModel')
	modelElem.Model = model
	if (tint != nil)
		modelElem.Tint = tint;
		
	renderable.AddElement(modelElem)
	return renderable
}

local function HudButton(modelPath, disabledPath)
{
	// Construct Appearance
	local model = NewObject('InterfaceModel')
	model.Path = modelPath
	model.X = 0
	model.Y = 0
	model.XScale = 70.0
	model.YScale = 77.0
	local disabledModel = NewObject('InterfaceModel')
	disabledModel.Path = disabledPath
	disabledModel.X = 0
	disabledModel.Y = 0
	disabledModel.XScale = 70.0
	disabledModel.YScale = 77.0
	local button = NewObject('Button')
	local style = NewObject('ButtonStyle')
	style.Width = 38
	style.Height = 38
	local regAppearance = NewObject('ButtonAppearance')
	regAppearance.Background = ModelRenderable(model)
	style.Normal = regAppearance
	local hoverAppearance = NewObject('ButtonAppearance')
	hoverAppearance.Background = ModelRenderable(model, GetColor('white_hover'))
	style.Hover = hoverAppearance
	local selectedAppearance = NewObject('ButtonAppearance')
	selectedAppearance.Background = ModelRenderable(disabledModel, GetColor('yellow'))
	style.Selected = selectedAppearance
	local disabledAppearance = NewObject('ButtonAppearance')
	disabledAppearance.Background = ModelRenderable(disabledModel)
	style.Disabled = disabledAppearance
	// Set Appearance
	button.SetStyle(style)
	return button
}

local function NavbarAction(hotspot)
{
	local obj = NavbarButton(hotspot, false)
	obj.Width = 33
	obj.Height = 33
	return obj
}

local function weapon_list_item(index, name)
{
	local li = NewObject("ListItem")
	li.ItemA = NewObject("Panel")
	li.ItemA.Width = 13;
	local ta = NewObject("TextBlock")
	ta.TextSize = 8
	ta.HorizontalAlignment = HorizontalAlignment.Right
	ta.TextColor = GetColor("text")
	ta.TextShadow = GetColor("black")
	ta.Fill = true
	ta.Text = tostring(index)
	li.ItemA.Children.Add(ta)
	li.ItemB = NewObject("Panel")
	local tb = NewObject("TextBlock")	
	tb.TextSize = 8
	tb.HorizontalAlignment = HorizontalAlignment.Left
	tb.TextColor = GetColor("text")
	tb.TextShadow = GetColor("black")
	tb.Fill = true
	tb.Strid = name
	tb.MarginX = 3
	li.ItemB.Children.Add(tb)
	return li;
}

local navbox = require 'navbox'

class hud : hud_Designer
{
    hud()
    {
		base();
        this.ManeuverButtons = {}
        local btns = Game.GetManeuvers()
        local container = navbox.GetNavbox(this.Widget, btns)
        local locX = navbox.GetStartX(btns) - 15
        local activeIDS = 0
        for (index, button in ipairs(btns)) {
            local obj = HudButton(button.ActiveModel, button.InactiveModel)
            this.ManeuverButtons[button.Action] = obj
            obj.Anchor = AnchorKind.TopCenter
            obj.X = locX
            locX = locX + navbox.XSpacing + 10
            obj.Y = navbox.OffsetY
            if (button.Action != activeids)
                obj.OnClick(() => Game.HotspotPressed(button.Action));
            else
                activeIDS = index;
            container.AddChild(obj)
        }
		local weaplist = this.Elements.weapons_list;
		for (index, weapon in ipairs(Game.GetWeapons())) {
			weaplist.Children.Add(weapon_list_item(index, weapon.Strid));
		}
        this.UpdateManeuverState()
		local e = this.Elements;
        e.chatbox.OnTextEntered((category, text) => Game.ChatEntered(category, text));
		this.ContactList = Game.GetContactList();
		e.contactlistview.SetData(this.ContactList);
		this.HudFilters = {
			important = e.filter_important,
			ship = e.filter_ship,
			station = e.filter_station,
			loot = e.filter_loot,
			all = e.filter_all
		};
		e.filter_important.OnClick(() => this.FilterSelected("important"));
		e.filter_ship.OnClick(() => this.FilterSelected("ship"));
		e.filter_station.OnClick(() => this.FilterSelected("station"));
		e.filter_loot.OnClick(() => this.FilterSelected("loot"));
		e.filter_all.OnClick(() => this.FilterSelected("all"));
		this.ContactList.SetFilter("important");
	    e.chat.Chat = Game.GetChats()
		e.nnobj.Visible = false;
		
		e.showwireframe.OnClick(() => {
			e.showwireframe.Selected = true;
			e.showcontactlist.Selected = false;
			e.contactlist.Visible = false;
			e.targetwireframe.Visible = true;
		});

		e.showcontactlist.OnClick(() => {
			e.showwireframe.Selected = false;
			e.showcontactlist.Selected = true;
			e.contactlist.Visible = true;
			e.targetwireframe.Visible = false;
		});

		this.Map = new mapwindow()
		this.InfoWindow = new infowindow()
		this.PlayerStatus = new playerstatus()
	    this.Map.InitMap()
		
		var windows = {
			{ e.nn_map, this.Map },
		    { this.Elements.nn_info, this.InfoWindow },
			{ this.Elements.nn_playerstatus, this.PlayerStatus }
		};
		this.WindowManager = new childwindowmanager(this.Widget, windows)
    }

	FilterSelected(filter)
	{
		for(k, v in pairs(this.HudFilters)) {
			v.Selected = filter == k
		}
		this.ContactList.SetFilter(filter)
	}
    
	ObjectiveUpdate(nnids)
	{
		if(nnids > 0) {
			PlaySound("ui_new_story_star");
			local e = this.Elements
			e.nnobj.FadeIn(1.0);
			e.nnobj.Strid = nnids;
			Timer(4, () => e.nnobj.FadeOut(1.0));
		} else {
			e.nnobj.FadeOut(1.0);
		}
	}

    Update(delta)
    {
        this.UpdateManeuverState()
	    local e = this.Elements
        e.speedText.Text = Game.Speed() + ""
        e.thrustText.Text = Game.ThrustPercent() + "%"
	    e.hullgauge.PercentFilled = Game.GetPlayerHealth()
	    e.powergauge.PercentFilled = Game.GetPlayerPower()
	    e.shieldgauge.PercentFilled = Game.GetPlayerShield()
		e.wireframe.SetWireframe(Game.SelectionWireframe())
	    local cruise = Game.CruiseCharge()
	    
	    if (cruise >= 0) {
		    e.cruisecharge.Text = StringFromID(STRID_CRUISE_CHARGING) + " - " + cruise + "%"
		    e.cruisecharge.Visible = true
	    } else {
		    e.cruisecharge.Visible = false
	    }
	    
	    if (Game.SelectionVisible()) {
		    local pos = Game.SelectionPosition()
		    e.selection.Visible = true
		    e.selection.X = pos.X - (e.selection.Width / 2.0)
		    e.selection.Y = pos.Y - (e.selection.Height / 2.0)
		    e.selection_name.Text = Game.SelectionName()
		    local health = Game.SelectionHealth()
		    local shield = Game.SelectionShield()
		    if (health >= 0) {
			    e.selection_health.Visible = true
			    e.selection_health.PercentFilled = health
		    } else {
			    e.selection_health.Visible = false
		    }
		    if (shield >= 0) {
			    e.selection_shield.Visible = true
			    e.selection_shield.PercentFilled = shield
		    } else {
			    e.selection_shield.Visible = false
		    }
			
			local color = GetColor('color_' + Game.SelectionReputation())
			
			e.sel_tl.Background.GetElement(0).Tint = color; 
			e.sel_tr.Background.GetElement(0).Tint = color; 
			e.sel_bl.Background.GetElement(0).Tint = color; 
			e.sel_br.Background.GetElement(0).Tint = color; 
	    } else {
		    e.selection.Visible = false
	    }
    }
    
    
    UpdateManeuverState()
    {
        local activeManeuver = Game.GetActiveManeuver()
	    local maneuversEnabled = Game.GetManeuversEnabled()
	    for (action, button in pairs(this.ManeuverButtons)) 
	    {
		    button.Selected = (activeManeuver == action)
		    button.Enabled = maneuversEnabled.Get(action)
	    }
    }
    
    Killed()
    {
        this.Elements.hudcontrols.Visible = false;
        OpenModal(new popup(STRID_GAME_OVER,STRID_YOU_ARE_DEAD, 'ok', () => Game.Respawn()));
    }
    
    
    Pause() => OpenModal(new pausemenu());
    Chatbox() => this.Elements.chatbox.Visible = true;
    Popup(title,contents,id) => OpenModal(new popup(title,contents,'ok', () => Game.PopupFinish(id)));
}




















