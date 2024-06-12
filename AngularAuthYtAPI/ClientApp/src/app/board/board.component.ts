import { Component, OnInit } from '@angular/core';
import { CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { TodoService } from '../services/todo.service';

@Component({
  selector: 'app-board',
  templateUrl: './board.component.html',
  styleUrls: ['./board.component.scss']
})
export class BoardComponent implements OnInit {
  plannedTasks: any[] = [];
  startedTasks: any[] = [];
  finishedTasks: any[] = [];

  constructor(private todoService: TodoService) {}

  ngOnInit() {
    this.loadTasks();
  }

  loadTasks() {
    this.todoService.getAllTasks().subscribe((tasks: any[]) => {
      tasks.forEach(task => {
        if (task.state === 0) {
          this.plannedTasks.push(task);
        } else if (task.state === 1) {
          this.startedTasks.push(task);
        } else if (task.state === 2) {
          this.finishedTasks.push(task);
        }
      });
    });
  }

  drop(event: CdkDragDrop<any[]>, status: string) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      const taskToMove = event.previousContainer.data[event.previousIndex];
      const newState = this.getStateFromString(status);

      this.todoService.updateTaskStatus(taskToMove.id, newState).subscribe(() => {
        transferArrayItem(
          event.previousContainer.data,
          event.container.data,
          event.previousIndex,
          event.currentIndex
        );
      });
    }
  }

  private getStateFromString(status: string): number {
    switch (status) {
      case 'planned':
        return 0;
      case 'started':
        return 1;
      case 'finished':
        return 2;
      default:
        return -1;
    }
  }
}
