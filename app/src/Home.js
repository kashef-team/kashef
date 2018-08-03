import React, { Component } from 'react';
import { withStyles } from '@material-ui/core/styles';
import { BrowserRouter as Router, Route, Link } from "react-router-dom";

import TopAppBar from './TopAppBar';
import BottomBar from './BottomBar';

const styles = {

};

class Home extends Component {
    render() {
      return (
        <div>
            <TopAppBar />
            <BottomBar />
        </div>
      );
    }
  }
  
  export default withStyles(styles)(Home);