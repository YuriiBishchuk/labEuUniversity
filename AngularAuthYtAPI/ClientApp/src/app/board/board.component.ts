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
  newTaskTitle: string = '';

  constructor(private todoService: TodoService) {}

  ngOnInit() {
    this.loadTasks();
  }

  loadTasks() {
    this.todoService.getAllTasks().subscribe((tasks: any[]) => {
      this.plannedTasks = tasks.filter(task => task.state === 0);
      this.startedTasks = tasks.filter(task => task.state === 1);
      this.finishedTasks = tasks.filter(task => task.state === 2);
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

  addTask(status: string) {
    const newTask = {
      title: this.newTaskTitle || 'Нове завдання',
      state: this.getStateFromString(status),
      description: '',
      userId: 1, // Приклад ID користувача
      index: this.getNextIndex(status)
    };

    this.todoService.createTask(newTask).subscribe(() => {
      this.newTaskTitle = ''; // Скидаємо поле для введення назви
      this.loadTasks(); // Оновлюємо список завдань
    });
  }

  private getNextIndex(status: string): number {
    switch (status) {
      case 'planned':
        return this.plannedTasks.length;
      case 'started':
        return this.startedTasks.length;
      case 'finished':
        return this.finishedTasks.length;
      default:
        return 0;
    }
  }
}
