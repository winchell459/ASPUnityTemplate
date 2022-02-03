﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGraphGenerator : ASPGenerator
{
    [SerializeField] int worldWidth;
    [SerializeField] int worldHeight;
    [SerializeField] int gateKeyCount;
    [SerializeField] int maxGatePerKey;
    [SerializeField] int minGatePerKey;
    [SerializeField] int bossGateKey;
    [SerializeField] int startRoom;
    [SerializeField] int timeout;
    [SerializeField] int cpus;

    //override protected void SATISFIABLE()
    //{
    //    map.DisplayMap(Solver.answerSet, mapKey);
    //    Debug.LogWarning("SATISFIABLE unimplemented");
    //}

    //override protected void UNSATISFIABLE()
    //{
    //    Debug.LogWarning("UNSATISFIABLE unimplemented");
    //}

    //override protected void TIMEDOUT()
    //{
    //    Debug.LogWarning("TIMEDOUT unimplemented");
    //}

    //override protected void ERROR()
    //{
    //    Debug.LogWarning("ERROR unimplemented");
    //}

    protected override string getASPCode()
    {

        return bidirectional_rules + grid_structure + gate_key_rules;
    }

    protected override void startGenerator()
    {
        string aspCode = getASPCode();
        string path = Clingo.ClingoUtil.CreateFile(aspCode);
        
        solver.maxDuration = timeout + 10;
        solver.Solve(path, $" -c max_width={worldWidth} -c max_height={worldHeight} -c start_room={startRoom} -c key_count={gateKeyCount} -c max_gate_type_count={maxGatePerKey} -c min_gate_type_count={minGatePerKey} -c boss_gate_type={bossGateKey} --parallel-mode {cpus} --time-limit={timeout}");
        waitingOnClingo = true;
    }

    string grid_structure = @"
            #const max_width = 4.
            #const max_height = 3.

            #const start_room = 1.


            width(1..max_width).
            height(1..max_height).
            roomID(1..max_width*max_height).
            1{room_grid(XX,YY, ID)}1 :- width(XX), height(YY), ID = (YY - 1) * max_width + XX.
            %1{room(RoomID)}1 :- roomID(RoomID).
            room(ID) :- room_grid(XX,YY,ID).

            %{door(RoomID1, RoomID2)}1 :- room(RoomID1), room(RoomID2).
            {door(RoomID1, RoomID2)}1 :- room_grid(XX,YY, RoomID1), room_grid(XX+1, YY, RoomID2).
            {door(RoomID1, RoomID2)}1 :- room_grid(XX,YY, RoomID1), room_grid(XX-1, YY, RoomID2).
            {door(RoomID1, RoomID2)}1 :- room_grid(XX,YY, RoomID1), room_grid(XX, YY+1, RoomID2).
            {door(RoomID1, RoomID2)}1 :- room_grid(XX,YY, RoomID1), room_grid(XX, YY-1, RoomID2).

            %path(RoomID, Type) :- path(RoomIDSource, Type), door(RoomIDSource, RoomID), roomID(Type).
            %start(start_room).

            1{start(RoomID): room_grid(_,YY,RoomID), YY == 1}1.
            %path(RoomID, Type) :- roomID(RoomID), Type = RoomID.
            %:- room(RoomID), not path(RoomID, Type), roomID(Type).

        ";

    string bidirectional_rules = @"

            door_east_exit(RoomID) :- door(RoomID, NeighboorID), room_grid(XX, YY, RoomID), room_grid(XX + 1, YY, NeighboorID). 
            door_west_exit(RoomID) :- door(RoomID, NeighboorID), room_grid(XX, YY, RoomID), room_grid(XX - 1, YY, NeighboorID).
            door_north_exit(RoomID) :- door(RoomID, NeighboorID), room_grid(XX, YY, RoomID), room_grid(XX, Y2, NeighboorID), Y2<YY.
            door_south_exit(RoomID) :- door(RoomID, NeighboorID), room_grid(XX, YY, RoomID), room_grid(XX, Y2, NeighboorID), Y2 > YY.

            door_east_entrance(RoomID) :- door(NeighboorID, RoomID), room_grid(XX, YY, RoomID), room_grid(XX + 1, YY, NeighboorID). 
            door_west_entrance(RoomID) :- door(NeighboorID, RoomID), room_grid(XX, YY, RoomID), room_grid(XX - 1, YY, NeighboorID).
            door_north_entrance(RoomID) :- door(NeighboorID, RoomID), room_grid(XX, YY, RoomID), room_grid(XX, Y2, NeighboorID), Y2<YY.
            door_south_entrance(RoomID) :- door(NeighboorID, RoomID), room_grid(XX, YY, RoomID), room_grid(XX, Y2, NeighboorID), Y2 > YY.

            door_east(RoomID) :- door_east_exit(RoomID).
            door_east(RoomID) :- door_east_entrance(RoomID).
            door_west(RoomID) :- door_west_exit(RoomID).
            door_west(RoomID) :- door_west_entrance(RoomID).
            door_north(RoomID) :- door_north_exit(RoomID).
            door_north(RoomID) :- door_north_entrance(RoomID).
            door_south(RoomID) :- door_south_exit(RoomID).
            door_south(RoomID) :- door_south_entrance(RoomID).

            door_horizontal(RoomID) :- door_east(RoomID).
            door_horizontal(RoomID) :- door_west(RoomID).
            door_vertical(RoomID) :- door_north(RoomID).
            door_vertical(RoomID) :- door_south(RoomID).

            door_east_soft_lock(RoomID) :- door_east_exit(RoomID), not door_east_entrance(RoomID).
            door_east_soft_lock(RoomID) :- door_east_entrance(RoomID), not door_east_exit(RoomID).
            door_west_soft_lock(RoomID) :- door_west_exit(RoomID), not door_west_entrance(RoomID).
            door_west_soft_lock(RoomID) :- door_west_entrance(RoomID), not door_west_exit(RoomID).
            door_north_soft_lock(RoomID) :- door_north_exit(RoomID), not door_north_entrance(RoomID).
            door_north_soft_lock(RoomID) :- door_north_entrance(RoomID), not door_north_exit(RoomID).
            door_south_soft_lock(RoomID) :- door_south_exit(RoomID), not door_south_entrance(RoomID).
            door_south_soft_lock(RoomID) :- door_south_entrance(RoomID), not door_south_exit(RoomID).

            door_count(RoomID, Count) :- Count = {door_east(RoomID); door_west(RoomID); door_north(RoomID); door_south(RoomID)}, roomID(RoomID).
            door_soft_lock_count(RoomID, Count) :- Count = {door_east_soft_lock(RoomID); door_west_soft_lock(RoomID); door_north_soft_lock(RoomID); door_south_soft_lock(RoomID)}, roomID(RoomID).


            door_soft_locked(Source, Destination) :- door(Source, Destination), not door(Destination, Source).
    
            %:- door_count(RoomID, Count), roomID(RoomID), Count > 3.
    
            %#show door_count/2.

        %if room has a directional door, it can only have two doors
            :- door_soft_lock_count(RoomID, Count), Count > 0, door_count(RoomID, Count2), Count2 > 2.

        %world must have at least one directional door
            %:- {door_soft_lock_count(RoomID, Count) : roomID(RoomID), Count > 0} < 5.

        %neighboring door to a room with directional door cannot have a directional door
            :- door_soft_lock_count(RoomID, Count), Count > 1.

            %:- not door_east_soft_lock(_).
            %:- not door_west_soft_lock(_).
            %:- not door_north_soft_lock(_).
            %:- not door_south_soft_lock(_).

        ";

    string gate_key_rules = @"
            #const key_count = 3.
            #const max_gate_type_count = 2.

            keys_types(1..key_count).

            key_present(0).
            %key_present(1).
            %key_present(2).

            1{key(KeyID, RoomID) : roomID(RoomID)}1 :- keys_types(KeyID), not key_present(KeyID).
            min_gate_type_count {gate(KeyID, RoomID, RoomIDExit) : door(RoomID, RoomIDExit)} max_gate_type_count :- keys_types(KeyID).

      
            poi(RoomID, Count) :- roomID(RoomID), Count = {key(_, RoomID); start(RoomID); gate(_, RoomID, RoomIDLocked)}.

        %one point of interst per room
            :- poi(RoomID, Count), roomID(RoomID), Count > 1.

        %no point of interest can be next door to another
            :- poi(RoomID, Count), Count > 0, door(RoomID, RoomID2), poi(RoomID2, Count2), Count2 > 0.

        %no gates can have the same room gated
            :- gate(_, RoomID1, RoomID), gate(_, RoomID2, RoomID), RoomID1 != RoomID2.

            path_order(RoomID, T+1) :- door(RoomSourceID, RoomID), path_order(RoomSourceID, T), not gate(_, RoomID, _), T<max_width*max_height.
            path_order(RoomID, T+1) :- door(RoomSourceID, RoomID), path_order(RoomSourceID, T), gate(KeyID, RoomID, _), have_key(KeyID, T2), T>=T2, T<max_width*max_height.

            path_order(RoomID, 0) :- start(RoomID).

            agrogate(min; max;avg).
            path_order(RoomID, min, Min) :- Min = #min{T: path_order(RoomID,T)}, roomID(RoomID).

            %:- 2{path_order(RoomID, _)}, roomID(RoomID).
            :- {path_order(RoomID, _)}0, roomID(RoomID).

        %take the lowest path_order
            %:- path_order(RoomID, T), door(R1, RoomID), path_order(R1, T1), door(R2, RoomID), path_order(R2, T2), T1<T2, not T<T2.
            :- gate(_, RoomSourceID, RoomGatedID), path_order(RoomSourceID, min, T1), path_order(RoomGatedID, min, T2), not T1<T2.

            have_key(KeyID, T) :- path_order(RoomID, T), key(KeyID, RoomID), not key_present(KeyID).
            have_key(KeyID, 0) :- key_present(KeyID).
            %have_key(KeyID, RoomID, T+1) :- door(RoomSourceID, RoomID), have_key(KeyID, RoomSourceID, T).


        %% gated area --------------------------

            :- room(RoomID), {gated_order(RoomID,_); non_gated(RoomID); gated_gate(RoomID,_); gated_key(RoomID,_)} != 1.

        %% gated_order
            %gated_order(RoomID, GateRoomID) :- gate(KeyID,_,RoomID), GateRoomID = RoomID, not gated_key(RoomID,KeyRoomID), key(KeyID,KeyRoomID).
            gated_order(RoomID, GateRoomID) :- gate(KeyID,GateRoomID,RoomID), not gated_key(GateRoomID,KeyRoomID), key(KeyID,KeyRoomID).
            %gated_order(RoomID, GateRoomID) :- gate(KeyID,GateRoomID,RoomID).

            gated_order(RoomID, GateRoomID) :- gated_order(Source,GateRoomID), door(Source, RoomID), not door_soft_locked(Source, RoomID), not gate(_,RoomID,_).
            %gated_order(RoomID, GateRoomID) :- door(Source, RoomID), gated_order(Source,GateRoomID), gate(_,RoomID,GatedID), GateRoomID != RoomID, GatedID != Source.

        %% gated gate
            gated_gate(RoomID, GateRoomID) :- door(Source, RoomID), gated_order(Source,GateRoomID), gate(_,RoomID,GatedRoom), GateRoomID != RoomID, Source != GatedRoom.
            :- gate(_,GateRoomID,RoomID), not gated_order(RoomID, GateRoomID), not non_gated(RoomID).

            %:- gated_gate(RoomID, GateRoomID), gate(KeyID, GateRoomID,_), gate(K2, RoomID,_), KeyID == K2.

        %% gated key
            gated_key(RoomID, KeyRoomID) :- key(_,RoomID), KeyRoomID = RoomID.
            gated_key(RoomID, KeyRoomID) :- gated_key(Source, KeyRoomID), door(Source,RoomID), not gate(KeyID,Source,RoomID), key(KeyID, KeyRoomID).

            gated_key_check(RoomID, KeyRoomID) :- gated_key(RoomID, KeyRoomID), gate(KeyID,RoomID,_), key(KeyID, KeyRoomID).
            gated_key_check(RoomID, KeyRoomID) :- gated_key_check(Source,KeyRoomID), door(Source, RoomID), not gate(_,Source, RoomID).
            %gated_key_check(RoomID, KeyRoomID) :- gated_key_check(Source,KeyRoomID), door(Source, RoomID), not gate(_,Source, RoomID), key(KeyID, KeyRoomID), not gate_order(_,KeyID).
            %gated_key_check(RoomID, KeyRoomID) :- gated_key_check(Source,KeyRoomID), door(Source, RoomID), gate(_,Source, RoomID), key(KeyID, KeyRoomID), gate_order(_,KeyID).
            :- gated_key(RoomID, KeyRoomID), not gated_key_check(RoomID,KeyRoomID).


            %gated_area(RoomID) :- gate(_,Source,RoomID), gated_gate(Source, _).
            %gated_area(RoomID) :- gate(_,Source,RoomID), gated_order(RoomID,_).
            %gated_area(RoomID) :- door_soft_locked(RoomID,_).
            %gated_area(RoomID) :- door_soft_locked(_,RoomID).
            %gated_area(RoomID) :- gate(_,RoomID,_), gated_key(RoomID,_).

            gated_area(RoomID) :- gated_order(RoomID,_).
            gated_area(RoomID) :- gated_gate(RoomID,_).
            gated_area(RoomID) :- gated_key(RoomID,_).
       
            :- door_soft_locked(Source, Dest), non_gated(Source), non_gated(Dest).
            :- door_soft_locked(Source, Dest), gated_order(Source, GateRoomID), gated_order(Dest, GateRoomID).
            :- door_soft_locked(Source, Dest), gated_key(Source, KeyRoomID), gated_key(Dest, KeyRoomID).


        %% create gated_room for Unity
            gated_room(RoomID, KeyID) :- gated_order(RoomID, GateRoomID), gate(KeyID, GateRoomID,_), not boss_room(RoomID).

            
            
        %% not gated
            non_gated(RoomID) :- start(RoomID).
            %non_gated(RoomID) :- door(Source, RoomID), non_gated(Source), not gate(_,Source, RoomID), not door_soft_locked(Source, RoomID).
            non_gated(RoomID) :- door(Source, RoomID), non_gated(Source), not gated_area(RoomID).
           

        %% gate ordering 
            gate_order(0,0).
            %gate_order(1,2).
            %gate_order(2,3).
            
            gated_start(RoomID, KeyID ) :- gated_key(RoomID, KeyRoomID), key(KeyID,KeyRoomID), door_soft_locked(_,RoomID).
            %:- gate_order(K1,K2), gated_start(RoomID, K2), door_soft_locked(Source, RoomID), not gated_order(Source, GateRoomID), gate(K1,RoomID).
            %:- gate_order(K1,K2), gated_start(RoomID, K2), door_soft_locked(Source, RoomID), not gated_order(Source, GateRoomID), gate(KeyID, GateRoomID,_), K1 == KeyID.
            
            
            :- gate_order(K1,K2), gated_start(RoomID, K2), door_soft_locked(Source, RoomID), not gated_order(Source, _).
            :- gate_order(K1,K2), gated_start(RoomID, K2), door_soft_locked(Source, RoomID), gated_order(Source, GateRoomID), gate(KeyID, GateRoomID,_), K1 != KeyID.

        %% no gated area can have a directional connection into it unless it is same GateRoomID
            :- gated_order(RoomID, GateRoomID), door(Source, RoomID), not door(RoomID, Source), not gated_order(Source, GateRoomID), Source != GateRoomID.

        %% end gated area ---------------------------------


        % no softlocks on gated rooms
            :- gate(_, Source, Destination), door_soft_lock_count(Source, Count), Count > 0.
            :- gate(_, Source, Destination), door_soft_lock_count(Destination, Count), Count > 0.

            :- gate(_, Source, Destination), door_count(Source, Count), Count > 2.
            %:- gate(_, Source, Destination), door_count(Source, Count), Count > 4.

        % gated horizontally only
            gated_horizontally(0).
            %gated_horizontally(1).
            :- gate(KeyID,Source,_), door_north(Source), gated_horizontally(KeyID).
            :- gate(KeyID,Source,_), door_south(Source), gated_horizontally(KeyID).

        %% gated vertically only
            gated_vertically(0).
            %gated_vertically(2).
            :- gate(KeyID,Source,_), door_east(Source), gated_vertically(KeyID).
            :- gate(KeyID,Source,_), door_west(Source), gated_vertically(KeyID).

        %% boss room
            #const boss_room_max = 4.
            #const boss_room_min = 4.
            #const boss_gate_type = 2.
            boss_room_min {boss_room(RoomID): room(RoomID)} boss_room_max.

            :- boss_room(RoomID), {boss_room(Source):door(Source,RoomID)} == 0.
            boss_room_entrance(RoomID) :- boss_room(RoomID), door(Source, RoomID), not boss_room(Source).
            boss_room_exit(RoomID) :- boss_room(RoomID), door(RoomID, Source), not boss_room(Source).
            1{boss_room_start(RoomID) : boss_room(RoomID), not boss_room_entrance(RoomID)}1.
            :- {boss_room_entrance(_)} > 1.
            :- {boss_room_exit(_)} > 1.
            :- boss_room_exit(R1), boss_room_entrance(R2), R1 != R2.
            :- boss_room_entrance(RoomID), {door(Source,RoomID): not boss_room(Source)} > 1.

            :- boss_room(RoomID), {boss_room(Neighbor): door(Neighbor, RoomID)} < 2.
            :- boss_room(RoomID), not door_horizontal(RoomID).
            :- boss_room(RoomID), not door_vertical(RoomID).

            :- boss_room(RoomID), key(_,RoomID).
            :- boss_room(RoomID), gate(_,RoomID,_).

            gate(boss_gate_type, Source, RoomID) :- boss_room_entrance(RoomID), door(Source,RoomID), not boss_room(Source).

            gate_path_order(KeyID, T) :- T = #min{Order:gate(KeyID,RoomID,_),path_order(RoomID,Order) }, keys_types(KeyID).
            %:- path_order(BossKeyRoom, min, D1), gate_path_order(KeyID,D2), D1 < D2, key(boss_gate_type, BossKeyRoom), keys_types(KeyID), KeyID != boss_gate_type.
            
        ";
}
