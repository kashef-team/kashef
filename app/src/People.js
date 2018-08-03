import React, { Component } from 'react';
import { withStyles } from '@material-ui/core/styles';
import GridList from '@material-ui/core/GridList';
import GridListTile from '@material-ui/core/GridListTile';
import GridListTileBar from '@material-ui/core/GridListTileBar';

const styles = {
    root: {
        display: 'flex',
        flexWrap: 'wrap',
        justifyContent: 'space-around',
        overflow: 'hidden',
    },
    gridList: {
        width: 500,
        height: 450,
    },
    icon: {
        color: 'rgba(255, 255, 255, 0.54)',
    },
};

class People extends Component {
    render() {
      const { classes } = this.props;
    console.log('pople: ', this.props.people);
      return (
          <div className={classes.root}>
            <GridList cellHeight={180} className={classes.gridList}>
                {this.props.people.map(tile => (
                <GridListTile key={tile.id}>
                    <img src={tile.images[0].url} alt={tile.name} />
                    <GridListTileBar
                    title={tile.name}
                    subtitle={<span>Type: {tile.type} {tile.isMissing && <span>(Missing)</span>}</span>}
                    />
                </GridListTile>
                ))}
            </GridList>
          </div>
      );
    }
  }
  
  export default withStyles(styles)(People);