import React, { Component } from 'react';
import { Admin, Resource } from 'react-admin';

import restProvider from './sails-rest-provider';
import logo from './logo.svg';
import './App.css';

import { PersonList, PersonCreate, PersonEdit } from './persons';

const App = () => (
  <Admin
    dataProvider={restProvider('http://localhost:1337')}
    title="Kashef Dashboard"
  >
    <Resource name="person" list={PersonList} create={PersonCreate} edit={PersonEdit} />
  </Admin>
);

export default App;
