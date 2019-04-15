import { Injectable } from '@angular/core';
declare let alertify :any;

@Injectable({
  providedIn: 'root'
})
export class AlertifyService {

constructor() { }

//--> Méthode pour la boite de dialogue de confirmation
confirm(message:string,okCallback:()=>any){
  alertify.confirm(message,function(e){
    if(e){okCallback()}else{}
  });
}

//--> Méthode pour la méthode de dialogue Success
success(message:string){
  alertify.success(message);
}

//--> Méthode pour la méthode de dialogue Error
error(message:string){
  alertify.error(message);
}

//--> Méthode pour la méthode de dialogue Warning
warning(message:string){
  alertify.warning(message);
}

//--> Méthode pour la méthode de dialogue Message
message(message:string){
  alertify.message(message);
}


}

//--> Défault Boite de dialogue
alertify.defaults = {
  // dialogs defaults
  autoReset:true,
  basic:false,
  closable:true,
  closableByDimmer:true,
  frameless:false,
  maintainFocus:true, // <== global default not per instance, applies to all dialogs
  maximizable:true,
  modal:true,
  movable:true,
  moveBounded:false,
  overflow:true,
  padding: true,
  pinnable:true,
  pinned:true,
  preventBodyShift:false, // <== global default not per instance, applies to all dialogs
  resizable:true,
  startMaximized:false,
  transition:'pulse',

  // notifier defaults
  notifier:{
      // auto-dismiss wait time (in seconds)  
      delay:2,
      // default position
      position:'button-left',
      // adds a close button to notifier messages
      closeButton: false
  },

  // language resources 
  glossary:{
      // dialogs default title
      title:'AlertifyJS',
      // ok button text
      ok: 'OK',
      // cancel button text
      cancel: 'Cancel'            
  },

  // theme settings
  theme:{
      // class name attached to prompt dialog input textbox.
      input:'ajs-input',
      // class name attached to ok button
      ok:'ajs-ok',
      // class name attached to cancel button 
      cancel:'ajs-cancel'
  }
};

