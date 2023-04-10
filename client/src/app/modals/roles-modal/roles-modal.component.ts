import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})
export class RolesModalComponent implements OnInit {
  //aim: populate component from user management component
  username ='';
  availableRoles: any[] = [];
  selectedRoles: any[]
 = [];
  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
  }

  //change values of whether checkbox is checked or not based on the user's role.
  updateChecked(checkedValue: string) {
    const index = this.selectedRoles.indexOf(checkedValue);
    //if index is -1, it means that it is not inside the selected roles array.
    //remove one item at that index, else push this new value (checked role) into selected roles array.
    index !== -1? this.selectedRoles.splice(index, 1) : this.selectedRoles.push(checkedValue);
     
  }

}
