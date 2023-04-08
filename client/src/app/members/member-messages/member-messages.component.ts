import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm?: NgForm // access to message form
  @Input() username?: string;
  @Input() messages: Message[] = [];// inherit messages methods from member-detail component
  messageContent = '';


  constructor(private messageService: MessageService) { }

  ngOnInit(): void {

  }

  //Listens to database('html') and subscribe to observables, and do actions to it.
  sendMessage() {
    if (!this.username) return; // ensure username exists before sending message.
    this.messageService.sendMessage(this.username, this.messageContent).subscribe({
      next: message => {
        this.messages.push(message);
        this.messageForm?.reset(); // reset messages chatbox after messages has been sent.
      }
    })
  }


}
