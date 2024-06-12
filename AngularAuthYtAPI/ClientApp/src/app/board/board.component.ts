import { Component } from '@angular/core';
import { CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';

@Component({
  selector: 'app-board',
  templateUrl: './board.component.html',
  styleUrls: ['./board.component.scss']
})
export class BoardComponent {
  plannedTasks: string[] = ["Get to work", "Pick up groceries", "Go home", "Fall asleep"];
  startedTasks: string[] = ["Get up", "Brush teeth", "Take a shower", "Check e-mail", "Walk dog"];
  finishedTasks: string[] = [];

  drop(event: CdkDragDrop<string[]>, status: string) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex,
      );
    }
  }
}
